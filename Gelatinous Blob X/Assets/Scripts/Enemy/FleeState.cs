using UnityEngine;
using System.Collections;

public class FleeState : State<Enemy> {

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

    private GameObject target;
    private Vector3 targetVelocity;
    private Vector3 fleeVelocity;

    private Rigidbody selfBody;
    private Collider[] selfColliders;

    public FleeState(Enemy enemy, StateMachine<Enemy> fsm, GameObject target, float fleeDistance = FLEE_RADIUS)
        : base(enemy, fsm)
    {
        this.target = target;
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
        if (Vector3.Distance(Owner.transform.position, target.transform.position) < FLEE_RADIUS)
        {
            Debug.Log(Vector3.Distance(Owner.transform.position, target.transform.position));
            fleeVelocity = StaticMovementAlgorithms.KinematicFlee(selfBody, target.transform.position, SPEED);
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
