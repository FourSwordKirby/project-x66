using UnityEngine;
using System.Collections;

public class DynamicSeek : _DynamicMovement {
    public Vector3 target { get; set; }

    public DynamicSeek(Rigidbody rigidbody, Vector3 target, float acceleration) {
        this.rigidbody = rigidbody;
        this.target = target;
        this.acceleration = acceleration;
    }

    public override void update() {
        Vector3 targetDirection = target - rigidbody.position;
        Vector3 currentDirection = velocity;

        Vector3 accelerationDirection = (targetDirection - currentDirection).normalized;

        velocity += accelerationDirection * acceleration * Time.deltaTime;
    }
}

public class DynamicSeekComponent : _TargetedMovementComponent {

    private DynamicSeek dynamicSeek;

    void Awake() {
        dynamicSeek = new DynamicSeek(GetComponent<Rigidbody>(), getTargetPosition(), magnitude);
	}

    void Update() {
        dynamicSeek.acceleration = magnitude;
        dynamicSeek.target = getTargetPosition();
	}

    public override _MovementAlgorithm getMovementAlgorithm() {
        return dynamicSeek;
    }
}
