using UnityEngine;
using System.Collections;

public class MovementState : State<Player> {

    private Vector3 direction;
    private Rigidbody ownerBody;

    public MovementState(Player player, StateMachine<Player> fsm): base(player, fsm)
    {
        this.ownerBody = player.GetComponent<Rigidbody>();
        ownerBody.freezeRotation = true;
    }

    override public void Enter()
    {
        Debug.Log("entered movement state");
        return;
    }

    override public void Execute()
    {
        direction = Controls.getDirection();
        if (direction == Vector3.zero)
            Owner.ActionFsm.ChangeState(new IdleState(Owner, Owner.ActionFsm));
    }

    override public void FixedExecute()
    {
        float prevY = ownerBody.velocity.y;
        ownerBody.velocity = new Vector3(direction.x * Owner.movementSpeed, prevY, direction.z * Owner.movementSpeed);
    }

    override public void Exit()
    {
        Debug.Log("exited movement state");
        return;
    }
}
