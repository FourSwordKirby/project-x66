using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class CameraAlertState : State<SecurityCamera>
{

    private GameObject target;
    private bool increasing;

    private float alertThreshold;
    private float alertCounter;

    private float deactivationThreshold;
    private float deactivationCounter;

    private const float ARRIVAL_ANGLE = 1.0f;

    //TODO: Not quite sure how this differentiates from the camera suspcious state at the moment, probably relates to an overall alertness system.
    public CameraAlertState(SecurityCamera cam, StateMachine<SecurityCamera> fsm, GameObject target)
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
        Debug.Log("cam alert");
        Owner.PlayAlertSound();
        Owner.targetingCone.GetComponentInChildren<MeshRenderer>().material = Owner.alertLightMaterial;
    }

    override public void Execute()
    {
        Quaternion direction = Quaternion.LookRotation(target.transform.position - Owner.transform.position);

        if (deactivationCounter > deactivationThreshold/4)
        {
            if (Owner.parentSystem != null)
                Owner.parentSystem.lostSightofTarget(target.transform.position);
            Fsm.ChangeState(new CameraIdleState(Owner, Fsm));
        }
    }

    override public void FixedExecute()
    {
        if (Owner.rotationBody.GetComponentInChildren<Collider>().bounds.Intersects(target.GetComponentInChildren<Collider>().bounds))
        {
            Quaternion newRotation = Quaternion.LookRotation(target.transform.position - Owner.transform.position);
            Vector3 facingDirection = newRotation * Vector3.forward;

            //If we're outside our allowed rotation range, stop all turning
            if (!(facingDirection.x > Owner.xFacingRange.x && facingDirection.x < Owner.xFacingRange.y
                && facingDirection.y > Owner.yFacingRange.x && facingDirection.y < Owner.yFacingRange.y)
                && facingDirection.z > Owner.zFacingRange.x && facingDirection.z < Owner.zFacingRange.y)
            {
                return;
            }
            else
            {
                Quaternion direction = newRotation;
                Owner.rotationBody.transform.rotation = Quaternion.Lerp(Owner.rotationBody.transform.rotation, direction, 0.2f);
                this.deactivationCounter = 0;
            }
            return;
        }
        else
        {
            this.deactivationCounter += Time.deltaTime;
        }
        
    }

    override public void Exit()
    {
        Owner.StopSound();
    }
}
