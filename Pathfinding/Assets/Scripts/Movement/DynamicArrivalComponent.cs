using UnityEngine;
using System.Collections;

public class DynamicArrival : _DynamicMovement {
    public float approachRadius { get; set; }

    // Formula constants
    private float maxSpeed;
    private float maxTurn;
    private float diffCoeff;
    private float accelCoefficient;

    private Rigidbody self;
    private Transform target;

    public DynamicArrival(Rigidbody self, Transform target, float radius, float maxSpeed, float maxTurn, float diffCoeff, float accelCoeff) {
        this.self = self;
        this.target = target;
        this.approachRadius = radius;
        this.maxSpeed = maxSpeed;
        this.maxTurn = maxTurn;
        this.diffCoeff = diffCoeff;
        this.accelCoefficient = accelCoeff;
    }

    public override void update() {
        Vector3 targetDirection = target.position - self.position;

        float targetSpeed;
        if (targetDirection.sqrMagnitude < approachRadius * approachRadius)
        {
            targetSpeed = 0.0f;
        }
        else
        {
            targetSpeed = Mathf.Min(maxSpeed, diffCoeff * targetDirection.magnitude);
        }

        Vector3 acceleration = accelCoefficient * (Mathf.Max(0.0f, (targetSpeed - self.velocity.magnitude))) * targetDirection.normalized;
        velocity += acceleration * Time.deltaTime;

        Debug.DrawLine(self.position, self.position + velocity, Color.blue);
        Debug.DrawLine(self.position + velocity, self.position + velocity + acceleration, Color.red);
    }
}

public class DynamicArrivalComponent : _TargetedMovementComponent {
    [Header("Dynamic Arrival Settings")]
    public float approachRadius;

    private DynamicArrival dynamicArrival;

    void Awake() {
        dynamicArrival = null;
    }

    void Update() {
        //dynamicArrival.acceleration = magnitude;
        //dynamicArrival.target = getTargetPosition();
    }

    public override _MovementAlgorithm getMovementAlgorithm() {
        return dynamicArrival;
    }
}
