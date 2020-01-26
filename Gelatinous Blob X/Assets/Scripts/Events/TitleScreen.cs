using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour {

	// Update is called once per frame
	void Update () {
	    if(Controls.interactInputDown())
        {
            TransitionManager.Instance.FadeToDark(() =>
            {
                Application.LoadLevel("OpeningSequence");
            });
        }
	}
}
