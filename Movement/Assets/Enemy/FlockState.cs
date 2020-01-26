using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FlockState : State<Enemy> {

    /*Not sure how much of this is needed, all  commented out for now
    private const float MIN_TIME = 2.0f;
    private const float MAX_TIME = 5.0f;
    private const float SPEED = 0.70f;
    private const float TURN_SPEED = 0.1f;

    private Vector3 orientation;
    private float movementVariance;

    private float directionChangeTimer;
    */

    private GameObject target;
    private List<Enemy> group;

    private const float VELOCITY_MATCHING_WEIGHT = 0.3f;
    private const float SEPERATION_WEIGHT = 0.2f;
    private const float COHESION_WEIGHT = 0.5f;

    private const float COLLISION_RADIUS = 1.0f;

    private const float SPEED = 4.0f;
    private const float ACCEL = 0.1f;
    private const float AVOID_DETECTION_RADIUS = 5.0f;
    private const float AVOID_MARGIN = 0.7f;
    private const float FLOCK_PERIOD = 0.2f;

    private Vector3 targetVelocity;
    private Vector3 flockingVelocity;
    private float flockTimer;

    // Optimization
    private Collider[] selfColliders;

    public FlockState(Enemy enemy, StateMachine<Enemy> fsm, GameObject target, List<Enemy> group)
        : base(enemy, fsm)
    {
        this.target = target;
        this.group = group;
    }

    override public void Enter()
    {
        selfColliders = Owner.GetComponentsInChildren<Collider>();
        flockTimer = FLOCK_PERIOD;
    }

    override public void Execute()
    {
        flockTimer -= Time.deltaTime;
        if(flockTimer > 0.0f)
        {
            return;
        }

        Rigidbody selfBody = this.Owner.GetComponent<Rigidbody>();
        Vector3 velocityMatching = target.GetComponent<Rigidbody>().velocity;

        Vector3 separation = StaticMovementAlgorithms.KinematicFleeMultiple(this.Owner.GetComponent<Rigidbody>(),
            group.Select(x => x.transform.position).ToList<Vector3>(),
            SPEED,
            0.2f);

        Vector3 centerOfMass = new Vector3();
        foreach (Enemy member in group)
        {
            centerOfMass += member.transform.position;
        }
        centerOfMass = centerOfMass / group.Count;
        Vector3 displace = centerOfMass - selfBody.position;
        Vector3 cohesion = displace.magnitude * StaticMovementAlgorithms.KinematicSeek(this.Owner.GetComponent<Rigidbody>(), centerOfMass, SPEED).normalized;

        flockingVelocity = velocityMatching
            + separation
            +  cohesion;
        if (flockingVelocity.sqrMagnitude > SPEED * SPEED)
        {
            flockingVelocity = flockingVelocity.normalized * SPEED;
        }

        //Vector3 avoidanceVelocity = StaticMovementAlgorithms.KinematicAvoidObstacles(Owner.GetComponent<Rigidbody>(), LayerMask.GetMask("Obstacle"), 5.0f, 1.0f, SPEED);
        Vector3 avoidanceVelocity =
            CollisionPrediction.AvoidCollisions(Owner.gameObject, AVOID_DETECTION_RADIUS, AVOID_MARGIN, SPEED, LayerMask.GetMask("Obstacle"), selfColliders);
        avoidanceVelocity.y = 0;
        if (avoidanceVelocity != Vector3.zero)
        {
            targetVelocity = 0.4f * avoidanceVelocity + 0.6f * flockingVelocity;
        }
        else
        {
            targetVelocity = flockingVelocity;
        }
        targetVelocity = targetVelocity.normalized * SPEED;

        flockTimer = FLOCK_PERIOD;
    }

    override public void FixedExecute()
    {
        Rigidbody body = Owner.GetComponent<Rigidbody>();
        body.velocity = Vector3.Lerp(body.velocity, targetVelocity, ACCEL);

        if (targetVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion direction = Quaternion.LookRotation(targetVelocity);
            Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, direction, 0.1f);
        }
        //Quaternion direction = Quaternion.LookRotation(displace);
        //Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, direction, 0.03f);
    }

    override public void Exit()
    {
    }
}
