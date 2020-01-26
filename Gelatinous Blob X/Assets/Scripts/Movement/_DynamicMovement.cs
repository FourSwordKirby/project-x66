using UnityEngine;
using System.Collections;

public abstract class _DynamicMovement : _MovementAlgorithm {
    public float acceleration { get; set; }
    protected Vector3 velocity { get; set; }
    protected Quaternion orientation { get; set; }

    public virtual void update() {

    }

    public override Vector3 getVelocity() {
        return velocity;
    }
    public Quaternion getOrientation()
    {
        return orientation;
    }
}
