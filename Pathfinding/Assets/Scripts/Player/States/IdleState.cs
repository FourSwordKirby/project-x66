using UnityEngine;
using System.Collections;

public class IdleState : State<Player> {

    public IdleState(Player player, StateMachine<Player> fsm) : base(player, fsm)
    {
    }

    override public void Enter()
    {
        Debug.Log("entered idle state");
        return;
    }

    override public void Execute()
    {
        Vector3 input_direction = Controls.getDirection();
        if (input_direction != Vector3.zero)
        {
            Owner.ActionFsm.ChangeState(new MovementState(Owner, Owner.ActionFsm));
        }
        //if (Controls.actionInputDown())
        //    player.changeState(new ParryState(player));

        /* This is actually kind of bad since you can choose to lock on or not in many states
         * Fudge multiple fsms???
         * Goddamn it stephen
         * */
        /*
        if (Controls.lockonInputDown())
        {
            if (player.target == null)
            {
                player.target = GameManager.getTarget();
                GameManager.Camera.Target(player, player.target);
            }
            else
            {
                Debug.Log("here");
                player.target = null;
                GameManager.Camera.Target(player);
            }
        }*/
    }

    override public void FixedExecute()
    {
    }

    override public void Exit()
    {
        Debug.Log("exited idle state");
        return;
    }
}
