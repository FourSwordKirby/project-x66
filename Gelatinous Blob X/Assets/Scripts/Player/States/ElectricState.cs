using UnityEngine;
using System.Collections;

public class ElectricState : State<Player> {

    public ElectricNavpoint nextNode;
    public ElectricNavpoint previousNode;

    private Player player;

    public Rigidbody selfBody;

    private Vector3 targetVelocity;

    private TurnTransparent[] transparentObj;

    public const float ELECTRIC_SPEED = 10.0f;
    public const float ARRIVE_RADIUS = 0.1f;

    public const float STAMINA_COST_PER_SECOND = 5.0f;

    public ElectricState(Player player, StateMachine<Player> fsm, ElectricNavpoint entry) : base(player, fsm)
    {
        previousNode = entry;
        nextNode = entry.getFirstNavPoint();
        selfBody = player.GetComponent<Rigidbody>();

        transparentObj = GameObject.FindObjectsOfType<TurnTransparent>();

        entry.Trigger();
    }

    override public void Enter()
    {
        Debug.Log("entered electric state");
        Owner.SetLightningBody(true);
        Owner.DisableColliders();
        Owner.SetStaminaRecovery(false);
        Owner.Sounds.PlaySoundOnLoop(Owner.Sounds.ElectricCurrentSound, 0.05f);
        foreach (TurnTransparent t in transparentObj)
        {
            t.SetTransparent(true);
        }
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
                if (input_direction != Vector3.zero && Owner.stamina > 0.0f)
                {
                    nextNodeCandidate = nextNode.getNextNavPoint(input_direction, previousNode);
                    if(nextNodeCandidate == previousNode)
                        nextNodeCandidate = nextNode.getNextNavPoint(selfBody.velocity, previousNode);
                }
                else
                    nextNodeCandidate = nextNode.getNextNavPoint(selfBody.velocity, previousNode);

                previousNode = nextNode;
                nextNode = nextNodeCandidate;

                //Now we trigger any associated devices
                previousNode.Trigger();
            }
        }
        Owner.UseStamina(STAMINA_COST_PER_SECOND * Time.deltaTime);
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
        Owner.EnableColliders();
        Owner.SetLightningBody(false);
        Owner.SetStaminaRecovery(true);
        Owner.Sounds.StopSound();
        foreach (TurnTransparent t in transparentObj)
        {
            t.SetTransparent(false);
        }
        return;
    }
}
