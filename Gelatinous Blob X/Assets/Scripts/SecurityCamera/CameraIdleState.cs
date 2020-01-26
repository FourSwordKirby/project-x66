using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class CameraIdleState : State<SecurityCamera> {

    private GameObject pointToTarget;
    private int nextTargetIndex;
    private bool increasing;
    private Vector3 turningAxis;

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(Owner.transform.position, Owner.rotationBody.transform.forward));
    }


    private const float ARRIVAL_ANGLE = 3.0f;

    public CameraIdleState(SecurityCamera cam, StateMachine<SecurityCamera> fsm)
        : base(cam, fsm)
    {
        nextTargetIndex = 0;
        this.pointToTarget = cam.targetingPoints[nextTargetIndex];
        increasing = true;
        turningAxis = cam.rotationBody.transform.up;
    }

    override public void Enter()
    {
        Debug.Log("cam idle");
        Owner.targetingCone.GetComponentInChildren<MeshRenderer>().material = Owner.searchLightMaterial;
    }

    override public void Execute()
    {
        //look at the next target once we've reached one
        Vector3 newForward = pointToTarget.transform.position- Owner.rotationBody.transform.position;
        Quaternion direction = Quaternion.Slerp(Owner.rotationBody.transform.rotation, Quaternion.LookRotation(newForward), 2);

        //Calculates the axis we are rotating around
        turningAxis = new Vector3(direction.x, direction.y, direction.z).normalized;

        if (Vector3.Angle(Owner.rotationBody.transform.forward, newForward.normalized) < ARRIVAL_ANGLE)
        {
            if (!Owner.cycleTargets)
            {
                if (nextTargetIndex == 0)
                    increasing = true;
                else if (nextTargetIndex == Owner.targetingPoints.Count - 1)
                    increasing = false;

                if (increasing)
                    nextTargetIndex++;
                else
                    nextTargetIndex--;
            }
            else
            {
                if (nextTargetIndex == Owner.targetingPoints.Count - 1)
                    nextTargetIndex = 0;
                else
                    nextTargetIndex++;
            }
            this.pointToTarget = Owner.targetingPoints[nextTargetIndex];
        }
    }

    override public void FixedExecute()
    {
        //Debug.Log(turningAxis);
        //If we're not at the end and paused briefly
        Vector3 newForward = pointToTarget.transform.position- Owner.transform.position;
        Owner.rotationBody.transform.rotation = Quaternion.RotateTowards(Owner.rotationBody.transform.rotation, Quaternion.LookRotation(newForward), Owner.turnSpeed * Time.deltaTime);
        //Owner.rotationBody.transform.rotation *= Quaternion.AngleAxis(Owner.turnSpeed * Time.deltaTime, turningAxis);
    }

    override public void Exit()
    {
    }
}
