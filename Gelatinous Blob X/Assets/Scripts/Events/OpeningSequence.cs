using UnityEngine;
using System.Collections;

public class OpeningSequence : MonoBehaviour {

    public AudioClip BrokenGlassSound;
    public AudioClip DoorClose;

    private bool skip;

    void Awake()
    {
        skip = false;
    }

    void Start()
    {
        TransitionManager.Instance.SetScreenEmpty();
    }

    void Update()
    {
        if(!skip && Controls.interactInputDown())
        {
            skip = true;
            TransitionManager.Instance.FadeToDark(() =>
            {
                LoadGame();
            });
        }
    }

    public void PlayBrokenGlassSound()
    {
        AudioSource.PlayClipAtPoint(BrokenGlassSound, Camera.main.transform.position, 0.3f);
    }

    public void PlayRegularDoorClose()
    {
        AudioSource.PlayClipAtPoint(DoorClose, Camera.main.transform.position, 0.3f);
    }

    public IEnumerator FadeMusic()
    {
        AudioSource audio = GetComponent<AudioSource>();
        while(audio.volume > 0.0f)
        {
            audio.volume -= 0.01f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void LoadGame()
    {
        Application.LoadLevel("BasicStarterRoom");
    }
}
