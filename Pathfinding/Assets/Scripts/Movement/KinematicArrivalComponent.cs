using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;

public class KinematicArrival : _KinematicMovement {

    public float speed { get; set; }
    public Vector3 target { get; set; }
    public float approachRadius { get; set; }

    public KinematicArrival(Rigidbody rigidbody, Vector3 target, float speed, float approachRadius) {
        this.rigidbody = rigidbody;
        this.speed = speed;
        this.target = target;
        this.approachRadius = approachRadius;
    }

    public override Vector3 getVelocity() {
        Vector3 offset = target - rigidbody.position;

        if (offset.sqrMagnitude < approachRadius * approachRadius) {
            return Vector3.zero;
        }
        else {
            return offset * speed;
        }
    }
}

public class KinematicArrivalComponent : _TargetedMovementComponent {
    [Header("Kinematic Arrival Settings")]
    public float approachRadius;
    private KinematicArrival kinematicArrival;

    void Awake() {
        kinematicArrival = new KinematicArrival(GetComponent<Rigidbody>(), getTargetPosition(), magnitude, approachRadius);
    }

    void Update() {
        kinematicArrival.speed = magnitude;
        kinematicArrival.target = getTargetPosition();
    }

    public override _MovementAlgorithm getMovementAlgorithm() {
        return kinematicArrival;
    }
}
