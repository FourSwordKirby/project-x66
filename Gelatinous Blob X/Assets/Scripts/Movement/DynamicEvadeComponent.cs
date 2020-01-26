using UnityEngine;
using System.Collections;

public class DynamicEvade : _DynamicMovement {
    public Vector3 target { get; set; }
    public Vector3 targetVelocity { get; set; }

    public DynamicEvade(Rigidbody rigidbody, Vector3 target, Vector3 targetVelocity, float acceleration) {
        this.rigidbody = rigidbody;
        this.target = target;
        this.acceleration = acceleration;
    }

    public override void update() {
        Vector3 targetDirection = rigidbody.position - target;
        Vector3 currentDirection = velocity;

        if (currentDirection != Vector3.zero) {
            float approximateTimeToReachTarget = targetDirection.magnitude / currentDirection.magnitude;
            targetDirection += targetVelocity * approximateTimeToReachTarget;
        }

        Vector3 accelerationDirection = (targetDirection - currentDirection).normalized;

        velocity += accelerationDirection * acceleration * Time.deltaTime;
    }
}

public class DynamicEvadeComponent : _TargetedMovementComponent {

    private DynamicEvade dynamicEvade;

    void Awake() {
        dynamicEvade = new DynamicEvade(GetComponent<Rigidbody>(), getTargetPosition(), getTargetVelocity(), magnitude);
	}

    void Update() {
        dynamicEvade.acceleration = magnitude;
        dynamicEvade.target = getTargetPosition();
        dynamicEvade.targetVelocity = getTargetVelocity();
	}

    public override _MovementAlgorithm getMovementAlgorithm() {
        return dynamicEvade;
    }
}
