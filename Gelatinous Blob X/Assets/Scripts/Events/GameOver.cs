using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        TransitionManager.Instance.FadeToEmpty(null);
	}
	
    public void PlayAgainClick()
    {
        TransitionManager.Instance.FadeToDark(() =>
        {
            Application.LoadLevel("BasicStarterRoom");
        });
    }
}
