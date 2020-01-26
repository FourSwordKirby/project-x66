using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FleeMultipleState : State<Enemy> {

    public bool avoiding = false;

    private const float SPEED = 1.0f;
    private const float ACCEL = 0.1f;
    private const float FLEE_RADIUS = 6.0f;
    private const float AVOID_DETECTION_RADIUS = 5.0f;
    private const float AVOID_MARGIN = 2.0f;

    //handles wandering after we fled far enough
    private float movementVariance;
    private float directionChangeTimer;
    private const float MIN_TIME = 2.0f;
    private const float MAX_TIME = 5.0f;

    private List<Enemy> fleeGroup;
    private Vector3 targetVelocity;
    private Vector3 fleeVelocity;

    private Rigidbody selfBody;
    private Collider[] selfColliders;


     public FleeMultipleState(Enemy enemy, StateMachine<Enemy> fsm, List<Enemy> group)
        : base(enemy, fsm)
    {
        this.fleeGroup = group;
        selfBody = Owner.GetComponent<Rigidbody>();
    }

    override public void Enter()
    {
        selfColliders = Owner.GetComponentsInChildren<Collider>();
    }
    override public void Execute()
    {
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0.0f)
        {
            fleeVelocity = StaticMovementAlgorithms.KinematicWander(selfBody, SPEED, movementVariance, Owner.transform.forward);
            directionChangeTimer = Random.Range(MIN_TIME, MAX_TIME);
        }

        Vector3 avoidanceVelocity =
            CollisionPrediction.AvoidCollisions(Owner.gameObject, AVOID_DETECTION_RADIUS, AVOID_MARGIN, SPEED, LayerMask.GetMask("Obstacle"), selfColliders);

        //if we're in range of the thing we want to flee stop wandering
        float closestPosition = fleeGroup.Select(x => Mathf.Min(Vector3.Distance(Owner.transform.position, x.transform.position))).Aggregate((x, y) => Mathf.Min(x, y));
        if (closestPosition < FLEE_RADIUS)
        {
            //Debug.Log(Vector3.Distance(Owner.transform.position, target.transform.position));
            fleeVelocity = StaticMovementAlgorithms.KinematicFleeMultiple(this.Owner.GetComponent<Rigidbody>(),
                fleeGroup.Select(x => x.transform.position).ToList<Vector3>(),
                SPEED,
                1.0f).normalized;
        }

        if (avoidanceVelocity != Vector3.zero)
        {
            targetVelocity = 0.6f * fleeVelocity + 0.4f * avoidanceVelocity;
        }
        else
        {
            targetVelocity = fleeVelocity;
        }
        targetVelocity.y = 0.0f;
        targetVelocity = targetVelocity.normalized * SPEED;
    }

    override public void FixedExecute()
    {
        Owner.GetComponent<Rigidbody>().velocity = targetVelocity;

        Quaternion direction = Quaternion.LookRotation(targetVelocity);
        Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, direction, 0.1f);
    }

    override public void Exit()
    {
    }
}
