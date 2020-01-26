using UnityEngine;
using System.Collections;

public class DynamicFlee : _DynamicMovement {
    public Vector3 target { get; set; }

    public DynamicFlee(Rigidbody rigidbody, Vector3 target, float acceleration) {
        this.rigidbody = rigidbody;
        this.target = target;
        this.acceleration = acceleration;
    }

    public override void update() {
        Vector3 targetDirection = rigidbody.position - target;
        Vector3 currentDirection = velocity;

        Vector3 accelerationDirection = (targetDirection - currentDirection).normalized;

        velocity += accelerationDirection * acceleration * Time.deltaTime;
    }
}

public class DynamicFleeComponent : _TargetedMovementComponent {

    private DynamicFlee dynamicFlee;

    void Awake() {
        dynamicFlee = new DynamicFlee(GetComponent<Rigidbody>(), getTargetPosition(), magnitude);
    }

    void Update() {
        dynamicFlee.acceleration = magnitude;
        dynamicFlee.target = getTargetPosition();
    }

    public override _MovementAlgorithm getMovementAlgorithm() {
        return dynamicFlee;
    }
}
