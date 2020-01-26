using UnityEngine;
using System.Collections;

public class PlayerEyes : MonoBehaviour {

    public Rigidbody playerBody;

    private Vector3 targetDirection;
    private Vector3 direction;

    void Start() {
        direction = Vector3.right;
        targetDirection = new Vector3(1, 0, -1).normalized;
    }
    void Update() {
        Vector3 playerDirection = Vector3.ProjectOnPlane(playerBody.velocity, Vector3.up);
        if (playerDirection != Vector3.zero) {
            targetDirection = playerDirection.normalized;
        }

        direction = Vector3.Slerp(direction, targetDirection, Time.deltaTime * 5);
        Quaternion lookRotation = Quaternion.LookRotation(-direction, Vector3.up);
        transform.localRotation = lookRotation;
    }
}
