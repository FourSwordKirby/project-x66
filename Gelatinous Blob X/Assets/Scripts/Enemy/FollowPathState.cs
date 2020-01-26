using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPathState : State<Enemy> {

    private const float SPEED = 5.0f;
    private const float ACCEL = 0.1f;
    private const float ARRIVE_RADIUS = 2.0f;
    private const float AVOID_DETECTION_RADIUS = 5.0f;
    private const float AVOID_MARGIN = 2.0f;

    private List<GameObject> Path;
    private int PathIndex;
    private GameObject currentTarget;

    private Vector3 targetVelocity;

    private Rigidbody selfBody;
    private Collider[] selfColliders;

    public FollowPathState(Enemy enemy, StateMachine<Enemy> fsm, List<GameObject> Path)
        : base(enemy, fsm)
    {
        this.Path = Path;

/*        foreach (GameObject node in Path)
        {
            Debug.Log(node);
        }
*/
        selfBody = Owner.GetComponent<Rigidbody>();
        PathIndex = 0;
        //PathIndex = FindClosestPathIndex();
        Debug.Log("Following path of length " + Path.Count);
        currentTarget = Path[PathIndex];
    }

    private int FindClosestPathIndex() {
        float smallestDist = (Path[PathIndex].transform.position - this.Owner.transform.position).sqrMagnitude;
        int index = PathIndex;
        for(int i = PathIndex; i < Path.Count; ++i)
        {
            float currDist = (Path[i].transform.position - this.Owner.transform.position).sqrMagnitude;
            if (currDist < smallestDist)
            {
                smallestDist = currDist;
                index = i;
            }
        }
        return index;
    }

    override public void Enter()
    {
        selfColliders = Owner.GetComponentsInChildren<Collider>();
    }

    override public void Execute()
    {
        Vector3 targetDest;
        //if(Path.Count - PathIndex >= 3)
        //{
        //    targetDest = 0.7f * Path[PathIndex].transform.position
        //        + 0.2f * Path[PathIndex + 1].transform.position
        //        + 0.1f * Path[PathIndex + 2].transform.position;
        //}
        //else if (Path.Count - PathIndex >= 2)
        //{
        //    targetDest = 0.8f * Path[PathIndex].transform.position
        //        + 0.2f * Path[PathIndex + 1].transform.position;
        //}
        //else if (Path.Count - PathIndex >= 1)
        //{
        //    targetDest = Path[PathIndex].transform.position;
        //}
        //else
        //{
        //    targetDest = currentTarget.transform.position;
        //}
        targetDest = currentTarget.transform.position;
        Debug.DrawLine(targetDest, targetDest + Vector3.up * 1.0f, Color.red);

        Vector3 arriveVelocity = StaticMovementAlgorithms.KinematicArrive(selfBody, targetDest, 1.0f, ARRIVE_RADIUS);
        //Vector3 avoidanceVelocity =
        //    CollisionPrediction.AvoidCollisionsHelper(Owner.gameObject,
        //    AVOID_DETECTION_RADIUS,
        //    1.5f * Owner.BodyBounds.extents.magnitude,
        //    LayerMask.GetMask("Obstacle"),
        //    selfColliders);
        Vector3 avoidanceVelocity = CollisionPrediction.AvoidCollisions(Owner.gameObject, AVOID_DETECTION_RADIUS, AVOID_MARGIN, SPEED, LayerMask.GetMask("Obstacle"), selfColliders);
        avoidanceVelocity.y = 0.0f;

        if (avoidanceVelocity != Vector3.zero)
        {
            targetVelocity = 0.6f * arriveVelocity + 0.4f * avoidanceVelocity;
        }
        else
        {
            targetVelocity = arriveVelocity;
        }
        //targetVelocity.y = 0.0f;
        targetVelocity = targetVelocity.normalized * Owner.speed;
        //targetVelocity.y = 0.0f;
        //float avoidanceSpeed = avoidanceVelocity.magnitude;
        //avoidanceVelocity.y = 0.0f;
        //avoidanceVelocity = avoidanceVelocity.normalized * avoidanceSpeed;
        //targetVelocity = arriveVelocity + avoidanceVelocity;
        //if (targetVelocity.sqrMagnitude > 1.0f)
        //{
        //    targetVelocity = targetVelocity.normalized;
        //}
        //targetVelocity *= SPEED;
        //Debug.Log("chosen target velocity: " + targetVelocity);

        if (Vector3.Distance(Owner.transform.position, currentTarget.transform.position) < ARRIVE_RADIUS)
        {
            //invariant that the path has a unique set of targets
            PathIndex++;
            if (PathIndex < Path.Count)
            {
                currentTarget = Path[PathIndex];
            }
        }
    }

    override public void FixedExecute()
    {
        Owner.GetComponent<Rigidbody>().velocity = Vector3.Lerp(Owner.GetComponent<Rigidbody>().velocity, targetVelocity, 0.1f);

        if (targetVelocity.sqrMagnitude >= 0.01f)
        {
            Quaternion direction = Quaternion.LookRotation(targetVelocity);
            Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, direction, 0.1f);
        }
    }

    override public void Exit()
    {
    }
}
