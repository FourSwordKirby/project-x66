using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMovementBehaviour : MonoBehaviour {

    [Header("Reference Settings")]
    public _MovementComponent movementComponent;

    [Header("Gameplay Settings")]
    public float interpolationAmount;

    [SerializeField]
    private bool lockX;
    [SerializeField]
    private bool lockY;
    [SerializeField]
    private bool lockZ;

    private new Rigidbody rigidbody;
    private _MovementAlgorithm movementAlgorithm;

	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        movementAlgorithm = movementComponent.getMovementAlgorithm();
        Debug.Log(movementAlgorithm);
        Debug.Log("Hello2");
	}
	
	void FixedUpdate () {
        if (movementAlgorithm is _KinematicMovement) {
            Vector3 instantaneousVelocity = movementAlgorithm.getVelocity();
            Vector3 newVelocity = Vector3.Lerp(rigidbody.velocity, instantaneousVelocity, interpolationAmount * Time.deltaTime);
            newVelocity.Scale(getDimensionLocks());

            rigidbody.velocity = newVelocity;
            rigidbody.rotation = Quaternion.FromToRotation(Vector3.right, newVelocity);

            Debug.Log(newVelocity);
        }
        else if (movementAlgorithm is _DynamicMovement) {
            ((_DynamicMovement)movementAlgorithm).update();
            Vector3 newVelocity = movementAlgorithm.getVelocity();
            newVelocity.Scale(getDimensionLocks());

            rigidbody.velocity = newVelocity;
            rigidbody.rotation = Quaternion.FromToRotation(Vector3.right, newVelocity);
        }
        else if (movementAlgorithm is _CompoundMovement) {
            Vector3 newVelocity = movementAlgorithm.getVelocity();
            newVelocity.Scale(getDimensionLocks());

            rigidbody.velocity = newVelocity;
            //rigidbody.rotation = Quaternion.FromToRotation(Vector3.right, newVelocity);
        }
	}
    private Vector3 getDimensionLocks() {
        return new Vector3(lockX ? 0 : 1, lockY ? 0 : 1, lockZ ? 0 : 1);
    }
}
