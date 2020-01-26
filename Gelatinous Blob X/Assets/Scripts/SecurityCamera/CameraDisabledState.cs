using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CameraDisabledState : State<SecurityCamera> {
    private float disabledCounter;
    float currentRotation;

    public CameraDisabledState(SecurityCamera cam, StateMachine<SecurityCamera> fsm, float disableDuration)
        : base(cam, fsm)
    {
        disabledCounter = disableDuration;
        currentRotation = 0;
    }

    override public void Enter()
    {
        Debug.Log("cam disabled");
        Owner.gameObject.GetComponentInChildren<MeshCollider>().enabled = false;
        Owner.targetingCone.GetComponentInChildren<MeshRenderer>().material = Owner.disabledLightMaterial;
    }

    public override void Execute()
    {
        if (disabledCounter > 0)
            disabledCounter -= Time.deltaTime;
        else
            Fsm.ChangeState(new CameraIdleState(Owner, Fsm));
    }

    public override void FixedExecute()
    {
        if (currentRotation < 30.0f)
        {
            Owner.rotationBody.transform.Rotate(new Vector3(1, 0, 0));
            currentRotation += 1;
        }
        //Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, direction, Owner.turnSpeed);
    }

    override public void Exit()
    {
        Owner.gameObject.GetComponentInChildren<MeshCollider>().enabled = true;
    }
}

