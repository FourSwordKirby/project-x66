using UnityEngine;
using System.Collections;

public class KinematicFlee : _KinematicMovement {

    public Vector3 target { get; set; }

    public KinematicFlee(Rigidbody rigidbody, Vector3 target, float speed) {
        this.rigidbody = rigidbody;
        this.speed = speed;
        this.target = target;
    }

    public override Vector3 getVelocity() {

        Vector3 offset = rigidbody.position - target;

        if (offset == Vector3.zero) {
            offset = Vector3.left;
        }

        return offset.normalized * speed;
    }
}

public class KinematicFleeComponent : _TargetedMovementComponent {
    private KinematicFlee kinematicFlee;

    void Awake() {
        kinematicFlee = new KinematicFlee(GetComponent<Rigidbody>(), getTargetPosition(), magnitude);
    }

    void Update() {
        kinematicFlee.speed = magnitude;
        kinematicFlee.target = getTargetPosition();
    }

    public override _MovementAlgorithm getMovementAlgorithm() {
        return kinematicFlee;
    }
}
