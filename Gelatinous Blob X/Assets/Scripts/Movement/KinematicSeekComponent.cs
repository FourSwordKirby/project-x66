using UnityEngine;
using System.Collections;

public class KinematicSeek : _KinematicMovement {

    public Vector3 target { get; set; }

    public KinematicSeek(Rigidbody rigidbody, Vector3 target, float speed) {
        this.rigidbody = rigidbody;
        this.speed = speed;
        this.target = target;
    }

    public override Vector3 getVelocity() {
        return (target - rigidbody.position).normalized * speed;
    }
}

public class KinematicSeekComponent : _TargetedMovementComponent {
    private KinematicSeek kinematicSeek;

    void Awake() {
        kinematicSeek = new KinematicSeek(GetComponent<Rigidbody>(), getTargetPosition(), magnitude);
        Debug.Log("Hello1");
    }

    void Update() {
        kinematicSeek.speed = magnitude;
        kinematicSeek.target = getTargetPosition();
    }

    public override _MovementAlgorithm getMovementAlgorithm() {
        return kinematicSeek;
    }
}
