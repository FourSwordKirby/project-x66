using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EffectCanvas : MonoBehaviour {

    private Camera cam;
    private Enemy enemy;
    private float margin;

	// Use this for initialization
	void Start () {
        cam = Camera.main;
        enemy = this.transform.GetComponentInParent<Enemy>();
        margin = enemy.GetComponentInChildren<Collider>().bounds.extents.magnitude * 1.2f;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = margin * -cam.transform.forward + enemy.transform.position;
        this.transform.rotation = cam.transform.rotation;
	}
}
