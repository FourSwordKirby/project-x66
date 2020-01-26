using UnityEngine;
using System.Collections;

public class PlayerEyes : MonoBehaviour {

    public Vector3 targetDirection { private get; set; }

    private Vector3 direction = new Vector3(1, 0, -1).normalized;

    void Update() {
        direction = Vector3.Slerp(direction, targetDirection, Time.deltaTime * 5);
        transform.LookAt(transform.position - direction, Vector3.up);
    }


}
