using UnityEngine;
using System.Collections;

public class ElectricPowerupScript :MonoBehaviour {

    public AudioClip ElectricChargeupSound;

    private AudioSource audioSource;
    private float startingVolume;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        startingVolume = audioSource.volume;
    }

    void OnTriggerEnter(Collider col)
    {
        GameManager.Player.SetStaminaRecovery(true);
        GameManager.Player.SetLightningBody(true);
        GameManager.Player.GainStamina(100.0f);
        GameObject.FindObjectOfType<PlayerCamera>().Shake(0.5f, 1.5f);
        GameObject.FindObjectOfType<PlayerCamera>().Flash(Color.white);
        StartCoroutine(PlayFadingElectricChargeupSound());
    }

    public IEnumerator PlayFadingElectricChargeupSound()
    {
        audioSource.clip = ElectricChargeupSound;
        audioSource.Play();
        while(audioSource.volume > 0)
        {
            audioSource.volume -= startingVolume * Time.deltaTime / 1.5f;
            yield return new WaitForEndOfFrame();
        }
    }

    void OnDestroy()
    {
        GameManager.Player.SetLightningBody(false);
    }
}
