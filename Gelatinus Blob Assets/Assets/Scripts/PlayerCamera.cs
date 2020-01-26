using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
    public GameObject player;

    private Vector3 offset;
    void Start() {
        offset = transform.position - player.transform.position;
    }

    void Update() {
        transform.position = player.transform.position + offset;
    }
}
