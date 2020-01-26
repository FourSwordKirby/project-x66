using UnityEngine;
using System.Collections;

public class ElectricDevice : MonoBehaviour {

    public bool isDisabled;
    public float disabledTime;
    private float disabledCounter;

	// Use this for initialization
	void Start () {
        disabledCounter = 0;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("base class");
        
	}



    public void disable()
    {
        disabledCounter = disabledTime;
        isDisabled = true;
        throw new System.NotImplementedException();
    }

    public void decrementCounter()
    {
        if (disabledCounter > 0)
            disabledCounter -= Time.deltaTime;
        else
            isDisabled = false;
    }
}
