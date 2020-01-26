using UnityEngine;
using System.Collections;

public enum PlayerState {
    IDLE, MOVING
}

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

    public PlayerState state;
    public PlayerEyes eyes;
    public Vector3 verticalAxis, horizontalAxis;

    public float movementSpeed;

    private new Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        Vector3 movement = (verticalMovement * verticalAxis + horizontalMovement * horizontalAxis).normalized;

        if (movement == Vector3.zero) {
            state = PlayerState.IDLE;
        }
        else {
            state = PlayerState.MOVING;

            eyes.targetDirection = movement;
        }

        rigidbody.velocity = movement * movementSpeed;
    }
}
