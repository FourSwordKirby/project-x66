using UnityEngine;
using System.Collections;

public class OscillateUpDown : MonoBehaviour {

    public float amplitude;
    [HideInInspector]
    public float frequency;

    Vector3 startPosition;

	void Start () {
        startPosition = transform.position;
	}

	void Update () {
        transform.position = startPosition + Mathf.Sin(Time.time * 2 * Mathf.PI * frequency) * Vector3.up;
	}
}
