using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class CameraSuspiciousState : State<SecurityCamera> {

    private GameObject target;
    private bool increasing;

    private float alertThreshold;
    private float alertCounter;

    private float deactivationThreshold;
    private float deactivationCounter;

    private const float ARRIVAL_ANGLE = 1.0f;

    public CameraSuspiciousState(SecurityCamera cam, StateMachine<SecurityCamera> fsm, GameObject target)
        : base(cam, fsm)
    {
        this.target = target;
        this.alertCounter = 0.0f;
        this.deactivationCounter = 0.0f;

        this.alertThreshold = Owner.alertThreshold;
        this.deactivationThreshold = Owner.deactivationThreshold;
    }

    override public void Enter()
    {
        Debug.Log("cam suspect");
        Owner.targetingCone.GetComponentInChildren<MeshRenderer>().material = Owner.suspiciousLightMaterial;
    }

    override public void Execute()
    {
        Quaternion direction = Quaternion.LookRotation(target.transform.position - Owner.transform.position);

        //TODO: make our FSMs more robust and be able to suspend state?
        if (deactivationCounter > deactivationThreshold)
        {
            Fsm.ChangeState(new CameraIdleState(Owner, Fsm));
        }

        if (alertCounter > alertThreshold)
        {
            if(Owner.parentSystem != null)
                Owner.parentSystem.Alert(target);
            Fsm.ChangeState(new CameraAlertState(Owner, Fsm, target));
        }
    }

    override public void FixedExecute()
    {
        if (Owner.rotationBody.GetComponentInChildren<Collider>().bounds.Intersects(target.GetComponentInChildren<Collider>().bounds))
        {
            Quaternion newRotation = Quaternion.LookRotation(target.transform.position - Owner.transform.position);
            Vector3 facingDirection = newRotation * Vector3.forward;
            Debug.Log(facingDirection);

            //If we're outside our allowed rotation range, stop all turning
            if (!(facingDirection.x < Owner.xFacingRange.x && facingDirection.x > Owner.xFacingRange.y
                && facingDirection.y < Owner.yFacingRange.x && facingDirection.y > Owner.xFacingRange.y)
                && facingDirection.z < Owner.zFacingRange.x && facingDirection.z > Owner.zFacingRange.y)
            {
                return;
            }
            else
            {
                Debug.Log("I have you now");
                Quaternion direction = Quaternion.LookRotation(target.transform.position - Owner.transform.position);
                Owner.rotationBody.transform.rotation = Quaternion.Lerp(Owner.rotationBody.transform.rotation, direction, 0.2f);
                this.alertCounter += Time.deltaTime;
            }
            return;
        }
        else
            this.deactivationCounter += Time.deltaTime;
    }

    override public void Exit()
    {
    }
}
