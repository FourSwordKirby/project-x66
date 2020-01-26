using UnityEngine;
using System.Collections;

public class Controls {

    public static Vector3 getDirection()
    {
        float gamepadX = Input.GetAxis("Horizontal");
        float gamepadY = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(gamepadX, 0, gamepadY);
        if (direction.sqrMagnitude < 1) {
            return direction;
        }
        return direction.normalized;
    }

    public static bool interactInputDown()
    {
        return (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Interact"));
    }

    public static bool lockonInputDown()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public static bool attackInputDown()
    {
        return Input.GetMouseButtonDown(1);
    }
}
