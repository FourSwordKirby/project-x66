using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public abstract class _MovementComponent : MonoBehaviour {
    [Header("Movement Settings")]
    [Tooltip("This doesn't do anything for compound movements.")]
    public float magnitude;
    public abstract _MovementAlgorithm getMovementAlgorithm();
}