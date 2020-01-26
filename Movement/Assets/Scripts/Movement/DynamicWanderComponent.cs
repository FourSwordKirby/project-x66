using UnityEngine;
using System.Collections;

public class DynamicWander : _DynamicMovement {

    private Vector3 currentDirection;

    public DynamicWander(Rigidbody rigidbody, float acceleration) {
        this.rigidbody = rigidbody;
        this.acceleration = acceleration;

        currentDirection = Random.onUnitSphere;
    }

    public override void update() {
        float dX = (Mathf.Sqrt(Random.Range(0f, 1f)) - 0.5f) * Random.Range(-10f, 10f);
        float dY = (Mathf.Sqrt(Random.Range(0f, 1f)) - 0.5f) * Random.Range(-10f, 10f);
        float dZ = (Mathf.Sqrt(Random.Range(0f, 1f)) - 0.5f) * Random.Range(-10f, 10f);

        Vector3 targetDirection = currentDirection + new Vector3(dX, dY, dZ);

        Vector3 accelerationDirection = (targetDirection - currentDirection).normalized;

        velocity += accelerationDirection * acceleration * Time.deltaTime;
    }
}

public class DynamicWanderComponent : _MovementComponent {

    private DynamicWander dynamicWander;

    void Awake() {
        dynamicWander = new DynamicWander(GetComponent<Rigidbody>(), magnitude);
	}

    void Update() {
        dynamicWander.acceleration = magnitude;
	}

    public override _MovementAlgorithm getMovementAlgorithm() {
        return dynamicWander;
    }
}
