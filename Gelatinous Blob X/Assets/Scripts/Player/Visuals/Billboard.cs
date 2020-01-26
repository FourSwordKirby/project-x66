using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    private Camera mainCamera;

    void Start() {
        mainCamera = Camera.main;
    }

    void Update() {
        transform.LookAt(transform.position + mainCamera.transform.forward, mainCamera.transform.up);
    }
}
