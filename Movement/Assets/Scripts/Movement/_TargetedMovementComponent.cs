using UnityEngine;
using System.Collections;

public abstract class _TargetedMovementComponent : _MovementComponent {
    [Header("Target Settings")]
    [SerializeField]
    [Tooltip("The position that you are targeting.")]
    private Vector3 target;
    [SerializeField]
    [Tooltip("The transform of a GameObject that you are targeting. This will override \"Target\".")]
    private Transform optionalTarget;
    [SerializeField]
    [Tooltip("The rigidbody of a GameObject that you are targeting. This will override \"Target\" and \"Optional Target\".")]
    private Rigidbody optionalRigidbodyTarget;

    public Vector3 getTargetPosition() {
        return optionalRigidbodyTarget ? optionalRigidbodyTarget.position : optionalTarget ? optionalTarget.position : target;
    }

    public Vector3 getTargetVelocity() {
        return optionalRigidbodyTarget ? optionalRigidbodyTarget.velocity : Vector3.zero;
    }
}
