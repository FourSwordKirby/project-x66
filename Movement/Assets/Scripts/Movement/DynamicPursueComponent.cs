using UnityEngine;
using System.Collections;

public class DynamicPursue : _DynamicMovement {
    public Vector3 target { get; set; }
    public Vector3 targetVelocity { get; set; }

    public DynamicPursue(Rigidbody rigidbody, Vector3 target, Vector3 targetVelocity, float acceleration) {
        this.rigidbody = rigidbody;
        this.target = target;
        this.acceleration = acceleration;
    }

    public override void update() {
        Vector3 targetDirection = target - rigidbody.position;
        Vector3 currentDirection = velocity;

        if (currentDirection != Vector3.zero) {
            float approximateTimeToReachTarget = targetDirection.magnitude / currentDirection.magnitude;
            targetDirection += targetVelocity * approximateTimeToReachTarget;
        }

        Vector3 accelerationDirection = (targetDirection - currentDirection).normalized;

        velocity += accelerationDirection * acceleration * Time.deltaTime;
    }
}

public class DynamicPursueComponent : _TargetedMovementComponent {

    private DynamicPursue dynamicPursue;

    void Awake() {
        dynamicPursue = new DynamicPursue(GetComponent<Rigidbody>(), getTargetPosition(), getTargetVelocity(), magnitude);
	}

    void Update() {
        dynamicPursue.acceleration = magnitude;
        dynamicPursue.target = getTargetPosition();
        dynamicPursue.targetVelocity = getTargetVelocity();
	}

    public override _MovementAlgorithm getMovementAlgorithm() {
        return dynamicPursue;
    }
}
