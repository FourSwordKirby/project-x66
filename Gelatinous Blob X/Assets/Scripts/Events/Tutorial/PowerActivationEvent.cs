using UnityEngine;
using System.Collections;

public class PowerActivationEvent : MonoBehaviour {

    private PlayerCamera cam;
    private Player player;
    private AudioSource audioSource;
    private bool activated;

    [SerializeField]
    private SecurityDoor doorToUnlock;
    [SerializeField]
    private AudioClip chargeUpSound;
    [SerializeField]
    private AudioClip electricExplosionSound;

    void Awake()
    {
        activated = false;
        audioSource = this.GetComponent<AudioSource>();
    }

	void Start ()
    {
        cam = FindObjectOfType<PlayerCamera>();
        player = GameManager.Player;
	}
	
	void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<Player>() != null && !activated)
        {
            activated = true;
            StartCoroutine(PlayEvent());
        }
    }

    private IEnumerator PlayEvent()
    {
        player.SetLightningBody(true);
        cam.Shake();
        cam.Flash(Color.white, 2.0f);
        TutorialEventScripts.EnableAllCameras();
        TutorialEventScripts.SetLights(true);
        doorToUnlock.Unlock();
        audioSource.PlayOneShot(electricExplosionSound, 0.1f);
        yield return new WaitForSeconds(0.8f);
        audioSource.PlayOneShot(chargeUpSound, 0.1f);
        yield return new WaitForSeconds(1.0f);
        TutorialEventScripts.PlayBackgroundTrack();
        player.SetLightningBody(false);
    }
}
