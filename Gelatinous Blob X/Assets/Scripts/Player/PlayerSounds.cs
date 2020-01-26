using UnityEngine;
using System.Collections;

public class PlayerSounds : MonoBehaviour {

    public AudioClip BounceSound;
    public AudioClip ElectricCurrentSound;

    private AudioSource audio;

    private float volume;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        volume = 1.0f;
    }

    public void PlaySoundOnLoop(AudioClip sound, float desiredVolume = 1.0f)
    {
        if (audio.clip != sound)
        {
            audio.Stop();
            audio.clip = sound;
            audio.pitch = 1.0f;
            audio.volume = desiredVolume * volume;
            audio.Play();
        }
        else
        {
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        }
    }

    public void PlayBounceSound()
    {
        if(audio.clip != BounceSound)
        {
            audio.Stop();
            audio.clip = BounceSound;
            audio.pitch = 0.6f;
            audio.volume = 0.15f * volume;
            audio.Play();
        }
        else
        {
            if(!audio.isPlaying)
            {
                audio.Play();
            }
        }
    }

    public void StopSound()
    {
        audio.Stop();
    }

    public void SetVolume(float v)
    {
        volume = Mathf.Clamp(v, 0.0f, 1.0f);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
