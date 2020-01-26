using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReachGoalState : State<Enemy> {

    public bool avoiding = false;

    private const float SPEED = 5.0f;
    private const float ACCEL = 0.1f;
    private const float ARRIVE_RADIUS = 1.0f;
    private const float AVOID_DETECTION_RADIUS = 5.0f;
    private const float AVOID_MARGIN = 2.0f;

    private GameObject target;
    private Vector3 targetVelocity;
    private Rigidbody selfBody;
    private Collider[] selfColliders;

    
    public ReachGoalState(Enemy enemy, StateMachine<Enemy> fsm, GameObject target, float arrivalDistance = ARRIVE_RADIUS)
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
        Vector3 arriveVelocity = StaticMovementAlgorithms.KinematicArrive(selfBody, target.transform.position, SPEED, ARRIVE_RADIUS);
        Vector3 avoidanceVelocity =
            CollisionPrediction.AvoidCollisions(Owner.gameObject, AVOID_DETECTION_RADIUS, AVOID_MARGIN, SPEED, LayerMask.GetMask("Obstacle"), selfColliders);

        if(avoidanceVelocity != Vector3.zero)
        {
            targetVelocity = 0.6f * arriveVelocity + 0.4f * avoidanceVelocity;
        }
        else
        {
            targetVelocity = arriveVelocity;
        }
        targetVelocity.y = 0.0f;
        targetVelocity = targetVelocity.normalized * SPEED;
    }

    override public void FixedExecute()
    {
        selfBody.velocity = Vector3.Lerp(selfBody.velocity, targetVelocity, ACCEL);

        if (targetVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion direction = Quaternion.LookRotation(targetVelocity);
            Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, direction, 0.1f);
        }
    }

    override public void Exit()
    {
    }
}
