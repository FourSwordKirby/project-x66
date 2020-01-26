using UnityEngine;
using System.Collections;

public class DumbWanderState : State<Enemy> {

	private const float MIN_TIME = 2.0f;
    private const float MAX_TIME = 5.0f;
    private const float SPEED = 1.0f;
    private const float TURN_SPEED = 0.1f;

    //private Vector3 orientation;
    private float movementVariance;
    private float directionChangeTimer;

    private Vector3 targetVelocity;
    private Vector3 wanderVelocity;


    public DumbWanderState(Enemy enemy, StateMachine<Enemy> fsm, float movementVariance) : base(enemy, fsm)
    {
        this.movementVariance = movementVariance;
        directionChangeTimer = Random.Range(MIN_TIME, MAX_TIME);
    }

    override public void Enter()
    {
    }

    override public void Execute()
    {
        Rigidbody body = Owner.GetComponent<Rigidbody>();
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0.0f)
        {
            wanderVelocity = StaticMovementAlgorithms.KinematicWander(body, SPEED, movementVariance, Owner.transform.forward);
            directionChangeTimer = Random.Range(MIN_TIME, MAX_TIME);
        }
        targetVelocity = wanderVelocity;
        targetVelocity.y = 0.0f;
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
