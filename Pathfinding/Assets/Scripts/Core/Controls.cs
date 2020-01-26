using UnityEngine;
using System.Collections;

public class Controls {

    public static Vector3 getDirection()
    {
        float gamepadX = Input.GetAxis("Horizontal");
        float gamepadY = Input.GetAxis("Vertical");
        if (gamepadX != 0 || gamepadY != 0)
        {
            return new Vector3(gamepadX, 0, gamepadY).normalized;
        }


        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            return new Vector3(-1, 0, 1).normalized;
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W))
        {
            return new Vector3(-1, 0, -1).normalized;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A))
        {
            return new Vector3(1, 0, -1).normalized;
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
        {
            return new Vector3(1, 0, 1).normalized;
        }
        else if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            return new Vector3(0, 0, 1).normalized;
        }
        else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W))
        {
            return new Vector3(-1, 0, 0).normalized;
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A))
        {
            return new Vector3(0, 0, -1).normalized;
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
        {
            return new Vector3(1, 0, 0).normalized;
        }
        else
            return Vector3.zero;
    }

    public static bool interactInputDown()
    {
        return (Input.GetKeyDown(KeyCode.Space) || Input.GetButton("Interact"));
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
