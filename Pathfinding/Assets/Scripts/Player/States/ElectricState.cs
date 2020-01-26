using UnityEngine;
using System.Collections;

public class ElectricState : State<Player> {

    public ElectricNavpoint nextNode;
    public ElectricNavpoint previousNode;

    public Rigidbody selfBody;

    private Vector3 targetVelocity;
    private Collider selfCollider;

    public const float ELECTRIC_SPEED = 10.0f;
    public const float ARRIVE_RADIUS = 0.1f;

    public ElectricState(Player player, StateMachine<Player> fsm, ElectricNavpoint entry) : base(player, fsm)
    {
        previousNode = entry;
        nextNode = entry.getFirstNavPoint();
        selfBody = player.GetComponent<Rigidbody>();
        selfCollider = Owner.GetComponentInChildren<Collider>();
    }

    override public void Enter()
    {
        Debug.Log("entered electric state");
        selfCollider.enabled = false;
        return;
    }

    override public void Execute()
    {
        targetVelocity = StaticMovementAlgorithms.KinematicArrive(selfBody, nextNode.transform.position, ELECTRIC_SPEED, ARRIVE_RADIUS);

        /*
        if (Controls.getDirection() != Vector3.zero)
        {
            Debug.Log("Controller decided");
            Debug.DrawLine(nextNode.transform.position, nextNode.getNextNavPoint(Controls.getDirection()).transform.position);
        }
        else
        {
            Debug.Log("Velocity Decided");
            Debug.DrawLine(nextNode.transform.position, nextNode.getNextNavPoint(selfBody.velocity).transform.position);
        }
        */

        if(targetVelocity == Vector3.zero)
        {
            if (nextNode.isEndpoint)
                Owner.ActionFsm.ChangeState(new IdleState(Owner, Owner.ActionFsm));
            else
            {
                ElectricNavpoint nextNodeCandidate;

                Vector3 input_direction = Controls.getDirection();
                if (input_direction != Vector3.zero)
                {
                    nextNodeCandidate = nextNode.getNextNavPoint(input_direction);
                    if(nextNodeCandidate == previousNode)
                        nextNodeCandidate = nextNode.getNextNavPoint(selfBody.velocity);
                }
                else
                    nextNodeCandidate = nextNode.getNextNavPoint(selfBody.velocity);

                previousNode = nextNode;
                nextNode = nextNodeCandidate;
            }
        }
    }

    override public void FixedExecute()
    {
        selfBody.velocity = targetVelocity;
        /*
        if (targetVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion direction = Quaternion.LookRotation(targetVelocity);
            Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, direction, 0.1f);
        }
         */ 
    }

    override public void Exit()
    {
        Debug.Log("exited electric state");
        selfCollider.enabled = true;
        return;
    }
}
