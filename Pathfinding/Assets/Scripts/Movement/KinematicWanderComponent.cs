using UnityEngine;
using System.Collections;

public class KinematicWander : _KinematicMovement {
    private Vector3 orientationEulerAngles;

    public KinematicWander(Rigidbody rigidbody, float speed) {
        this.speed = speed;
        this.rigidbody = rigidbody;

        orientationEulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    public override Vector3 getVelocity() {
        float dX = (Mathf.Sqrt(Random.Range(0f, 1f)) - 0.5f) * Random.Range(-10f, 10f);
        float dY = (Mathf.Sqrt(Random.Range(0f, 1f)) - 0.5f) * Random.Range(-10f, 10f);
        float dZ = (Mathf.Sqrt(Random.Range(0f, 1f)) - 0.5f) * Random.Range(-10f, 10f);

        orientationEulerAngles += new Vector3(dX, dY, dZ);
        Vector3 direction = Quaternion.Euler(orientationEulerAngles) * Vector3.right;

        return speed * direction;
    }
}

public class KinematicWanderComponent : _MovementComponent {
    [Header("Wander Settings")]

    private KinematicWander kinematicWander;

    void Awake() {
        kinematicWander = new KinematicWander(GetComponent<Rigidbody>(), magnitude);
    }

    public override _MovementAlgorithm getMovementAlgorithm() {
        return kinematicWander;
    }
}
