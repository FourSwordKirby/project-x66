using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCollisionAvoidance : MonoBehaviour {

    public new Rigidbody rigidbody;
    public LayerMask obstacles;
    public float speed;
    public float radius;
	
	void Update () {
        Vector3 newVelocity = StaticMovementAlgorithms.KinematicAvoidObstacles(rigidbody, obstacles, 10, radius, speed);
        newVelocity = StaticMovementAlgorithms.GetInterpolatedVelocity(rigidbody, newVelocity, 1);
        rigidbody.velocity = newVelocity;
        rigidbody.rotation = StaticMovementAlgorithms.GetOrientation(newVelocity, Vector3.forward);
	}
}
