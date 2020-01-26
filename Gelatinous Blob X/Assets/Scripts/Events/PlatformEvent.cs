using UnityEngine;
using System.Collections;

public class PlatformEvent : MonoBehaviour {

    private bool activated = false;

    void Start()
    {
    }

	// Update is called once per frame
	void Update () {
        if(activated)
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 5, 0);
	    if(this.transform.position.y > 40)
        {
            TransitionManager.Instance.FadeToDark(() => {
                Application.LoadLevel("OpeningSequence");
            });
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Player>() != null)
        {
            col.gameObject.GetComponent<Player>().enabled = false;
            col.gameObject.GetComponent<Player>().GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.GetComponent<Rigidbody>().isKinematic = false;
            //this.transform.position += new Vector3(0, 1, 0);
            activated = true;
        }
    }
}
