using UnityEngine;
using System.Collections.Generic;

public class SeekTargetWithPathState : State<Enemy> {
    
    //private const float SPEED = 5.0f;
    private const float ACCEL = 0.1f;
    private const float ARRIVE_RADIUS = 2.0f;
    private const float AVOID_DETECTION_RADIUS = 3.0f;
    private const float AVOID_MARGIN = 1.5f;
    private const float REPATH_TIMER = 1.0f;
    private const float REPATH_THRESHOLD = 2.0f;
    private const int REPATH_WAIT_MAX = 6;

    private GameObject target;
    private List<Vector3> path;
    private int pathIndex;
    private Vector3 pathPointTarget;
    private Vector3 targetLastPosition;
    private int repathWait;

    private Vector3 targetVelocity;

    private Rigidbody selfBody;
    private Collider[] selfColliders;

    public SeekTargetWithPathState(Enemy enemy, StateMachine<Enemy> fsm, GameObject target)
        : base(enemy, fsm)
    {
        this.target = target;
        this.selfBody = Owner.GetComponent<Rigidbody>();
        this.pathIndex = 0;
        this.pathPointTarget = target.transform.position;
    }


    override public void Enter()
    {
        this.selfColliders = Owner.GetComponentsInChildren<Collider>();
        GeneratePathToTarget();
    }

    override public void Execute()
    {
        Vector3 selfPos = Owner.transform.position;
        Vector3 targetPos = target.transform.position;

        if(Vector3.Distance(targetPos, selfPos) < ARRIVE_RADIUS)
        {
            targetVelocity = Vector3.zero;
            return;
        }

        Vector3 arrivePoint;
        if (path == null)
        {
            if(Mathf.Abs(selfPos.y - targetPos.y) < 0.1f &&
                !Physics.Linecast(selfPos, targetPos, LayerMask.GetMask("Obstacle")))
            {
                // If we're on the same y-plane, and the target is visible
                // Just target the enemy
                arrivePoint = targetPos;
                targetLastPosition = targetPos;
            }
            else
            {
                // Otherwise, find a path.
                GeneratePathToTarget();

                arrivePoint = pathPointTarget;
            }
        }
        else
        {
            // If the target has moved significantly, create a new path.
            if (Vector3.Distance(targetLastPosition, targetPos) > REPATH_THRESHOLD)
            {
                if(repathWait <= 0)
                {
                    GeneratePathToTarget();
                    repathWait = Random.Range(0, REPATH_WAIT_MAX);
                }
                else
                {
                    repathWait -= 1;
                }
            }

            // Otherwise, keep following our current path.
            else
            {
                // If we've reached our path way point, get the next if it exists
                if (Vector3.Distance(selfPos, pathPointTarget) < ARRIVE_RADIUS)
                {
                    pathIndex++;
                    if (pathIndex < path.Count)
                    {
                        pathPointTarget = path[pathIndex];
                    }
                    else
                    {
                        path = null;
                        pathPointTarget = targetPos;
                    }
                }
            }
            arrivePoint = pathPointTarget;
        }
        Debug.DrawLine(arrivePoint, arrivePoint + Vector3.up * 1.0f, Color.red);

        Vector3 arriveVelocity = StaticMovementAlgorithms.KinematicArrive(selfBody, arrivePoint, 1.0f, ARRIVE_RADIUS);
        Vector3 avoidanceVelocity = CollisionPrediction.AvoidCollisionsHelper(Owner.gameObject, AVOID_DETECTION_RADIUS, AVOID_MARGIN, LayerMask.GetMask("Obstacle"), selfColliders);
        avoidanceVelocity.y = 0.0f;

        if (avoidanceVelocity != Vector3.zero)
        {
            targetVelocity = targetVelocity + avoidanceVelocity * Owner.speed;
            if(targetVelocity.sqrMagnitude > Owner.speed * Owner.speed)
            {
                targetVelocity = targetVelocity.normalized * Owner.speed;
            }
        }
        else
        {
            targetVelocity = Vector3.Lerp(targetVelocity, arriveVelocity * Owner.speed, 0.2f);
        }
        if(Owner.gameObject == UnityEditor.Selection.activeGameObject)
        {
            Debug.Log(Owner.name + " has target vel " + targetVelocity + " arrive vel: " + arriveVelocity + " avoid vel : " + avoidanceVelocity);
        }
    }

    private float NotOnFireCost(INavCell source, INavCell target)
    {
        float distance = Vector3.Distance(source.center, target.center);
        if (target.properties.ContainsKey("OnFire"))
        {
            //Debug.Log("burning");
            return 30000.0f + distance;
        }
        else
            return distance;
    }

    private float OnFireCost(INavCell source, INavCell target)
    {
        float distance = Vector3.Distance(source.center, target.center);
        return distance;
    }

    private void GeneratePathToTarget()
    {
        //Debug.Log(Owner.gameObject.name + " is generating a path to " + target.name);
        if (!Owner.onFire)
        {
            path = PathFinding.generatePath(Owner.gameObject, target, NotOnFireCost);
        }
        else
        {
            path = PathFinding.generatePath(Owner.gameObject, target, OnFireCost);
        }
        pathIndex = 0;
        pathPointTarget = path[0];
        targetLastPosition = target.transform.position;
    }

    override public void FixedExecute()
    {
        Owner.GetComponent<Rigidbody>().velocity = targetVelocity;
        //Owner.GetComponent<Rigidbody>().velocity = Vector3.Lerp(Owner.GetComponent<Rigidbody>().velocity, targetVelocity, 0.1f);

        if (targetVelocity.sqrMagnitude >= 0.01f)
        {
            Quaternion direction = Quaternion.LookRotation(Owner.GetComponent<Rigidbody>().velocity, Vector3.up);
            Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, direction, 0.1f);
        }
    }

    override public void Exit()
    {
    }
}
