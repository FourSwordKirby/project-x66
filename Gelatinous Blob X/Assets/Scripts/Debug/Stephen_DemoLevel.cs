using UnityEngine;
using System.Collections;

public class Stephen_DemoLevel : MonoBehaviour {

    private Camera playerCam;
    private Camera overviewCam;

    private Vector3 closeLoc;
    private Vector3 originalLoc;
    private Player p;

	// Use this for initialization
	void Start () {
        foreach (Camera cam in Camera.allCameras)
        {
            if(cam.name == "Player Camera")
            {
                playerCam = cam;
            }
            else if(cam.name == "Overview Camera")
            {
                overviewCam = cam;
            }
        }
        //overviewCam.enabled = false;
        p = GameObject.FindObjectOfType<Player>();
        originalLoc = playerCam.transform.position - p.transform.position;
        closeLoc = (playerCam.transform.position - p.transform.position).normalized * 5.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            p.TakeDamage(1);
        }
	}
}
