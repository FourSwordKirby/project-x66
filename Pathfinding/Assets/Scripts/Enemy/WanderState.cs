using UnityEngine;
using System.Collections;

public class WanderState : State<Enemy> {

    private const float MIN_TIME = 2.0f;
    private const float MAX_TIME = 5.0f;
    private const float SPEED = 1.0f;
    private const float TURN_SPEED = 0.1f;

    private const float AVOID_DETECTION_RADIUS = 2.0f;
    private const float AVOID_MARGIN = 1.0f;

    //private Vector3 orientation;
    private float movementVariance;
    private float directionChangeTimer;

    private Vector3 targetVelocity;
    private Vector3 wanderVelocity;


    public WanderState(Enemy enemy, StateMachine<Enemy> fsm, float movementVariance) : base(enemy, fsm)
    {
        this.movementVariance = movementVariance;
        directionChangeTimer = 0.0f;// Random.Range(MIN_TIME, MAX_TIME);
    }

    override public void Enter()
    {
        //Rigidbody body = Owner.GetComponent<Rigidbody>();
        //wanderVelocity = Vector3.zero;
        //wanderVelocity = StaticMovementAlgorithms.KinematicWander(body, SPEED, movementVariance, Owner.transform.forward);
    }

    override public void Execute()
    {
        //Initialization of basic parameters
        Rigidbody body = Owner.GetComponent<Rigidbody>();

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0.0f)
        {
            wanderVelocity = StaticMovementAlgorithms.KinematicWander(body, SPEED, movementVariance, Owner.transform.forward);
            directionChangeTimer = Random.Range(MIN_TIME, MAX_TIME);
        }
        
        //Calculates the velocity for not colliding into things
        Vector3 avoidanceVelocity = CollisionPrediction.AvoidCollisions(Owner.gameObject, AVOID_DETECTION_RADIUS, AVOID_MARGIN, SPEED, LayerMask.GetMask("Obstacle"));
        //StaticMovementAlgorithms.KinematicAvoidObstacles(body, LayerMask.GetMask("Obstacle"), 5.0f, 1.0f, SPEED);
        avoidanceVelocity.y = 0;        
        //Vector3 targetVelocity = new Vector3();
        if (avoidanceVelocity != Vector3.zero)
        {
            targetVelocity = 0.6f * wanderVelocity 
                           + 0.4f * avoidanceVelocity;
            targetVelocity = targetVelocity.normalized * SPEED;
            wanderVelocity = targetVelocity;//avoidanceVelocity;
        }
        else
        {
            targetVelocity = wanderVelocity;
        }

        targetVelocity.y = 0.0f;

        targetVelocity = targetVelocity.normalized * SPEED;

        //Debug.Log("wander " + wanderVelocity);
        //Debug.Log("avoid " + avoidanceVelocity);

    }

    override public void FixedExecute()
    {
        //Vector3 next = Vector3.Lerp(Owner.GetComponent<Rigidbody>().velocity, targetVelocity, Time.deltaTime);
        Owner.GetComponent<Rigidbody>().velocity = targetVelocity;

        Quaternion direction = Quaternion.LookRotation(targetVelocity);
        Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, direction, 0.1f);

        //Debug.Log("target" + targetVelocity);

        //Debug.Log(ownerRigidBody.velocity);
        //Vector3 interped = Vector3.MoveTowards(ownerRigidBody.velocity, targetVelocity, 0.2f);//StaticMovementAlgorithms.GetInterpolatedVelocity(ownerRigidBody, targetVelocity, 5.0f);
        //Debug.Log("interped:" + interped);
        //ownerRigidBody.velocity = interped;

        //Owner.transform.rotation = StaticMovementAlgorithms.GetOrientation(ownerRigidBody.velocity, Vector3.forward);

        /*
        if (targetVelocity != Vector3.zero)
            ownerRigidBody.velocity = StaticMovementAlgorithms.GetInterpolatedVelocity(ownerRigidBody, targetVelocity, 5.0f);
        */
        
        // Changes speed
        //Owner.GetComponent<Rigidbody>().velocity = SPEED * Owner.transform.forward;

        //Changes orientation
        //Quaternion targetOrientation = Quaternion.Lerp(Owner.transform.rotation, Quaternion.LookRotation(orientation), TURN_SPEED);
        //Owner.transform.rotation = targetOrientation;
    }

    override public void Exit()
    {
    }
}
