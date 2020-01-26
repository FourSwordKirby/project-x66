using UnityEngine;
using System.Collections;

public abstract class _MovementAlgorithm {
    protected Rigidbody rigidbody;

    public Vector3 position { get { return rigidbody.position; } }

    public virtual Vector3 getVelocity() {
        return Vector3.zero;
    }
}