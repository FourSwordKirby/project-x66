using UnityEngine;
using System.Collections;

public class TutorialEventScripts : MonoBehaviour {

    protected static TutorialEventScripts instance;
    public static TutorialEventScripts Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TutorialEventScripts>();

                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(TutorialEventScripts) +
                       " is needed in the scene, but there is none.");
                }
            }

            return instance;
        }
    }

    // Specifics
    [SerializeField]
    private SecurityDoor testChamberDoor;
    [SerializeField]
    private SecurityDoor powerActivationRoomDoor;

    public AudioClip BackgroundTrack;


    // Programmatically acquired
    private SecurityCamera[] allCams;
    private SecurityDoor[] allDoors;
    private AudioSource audioSource;

    private Light[] allLights;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("A TutorialEventScript object already exists!");
            Destroy(this.gameObject);
            return;
        }
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = BackgroundTrack;
    }

	void Start ()
    {
        // Get all needed refs
        allCams = FindObjectsOfType<SecurityCamera>();
        allDoors = FindObjectsOfType<SecurityDoor>();
        allLights = FindObjectsOfType<Light>();

        // Initial level setup
        DisableAllCameras();
        CloseAndLockAllDoors();
        testChamberDoor.Unlock();
        powerActivationRoomDoor.Unlock();
        SetLights(false);

        //TransitionManager.Instance.SetScreenDark();
        TransitionManager.Instance.FadeToEmpty(null);

        GameManager.Player.SetStaminaRecovery(false);
        GameManager.Player.UseStamina(100000.0f);
	}
	
    public static void CloseAndLockAllDoors()
    {
        foreach(SecurityDoor door in instance.allDoors)
        {
            door.Lock();
            //door.Close();
        }
    }

    public static void DisableAllCameras(float duration = 0.0f)
    {
        float time = duration <= 0.0f ? 1000000000.0f : duration;
        foreach (SecurityCamera cam in instance.allCams)
        {
            cam.DisableCamera(time);
        }
    }

    public static void EnableAllCameras()
    {
        foreach(SecurityCamera cam in instance.allCams)
        {
            cam.ForceEnableCamera();
        }
    }

    public static void SetLights(bool on)
    {
        foreach (Light l in instance.allLights)
        {
            l.gameObject.SetActive(on);
        }
    }

    public static void PlayBackgroundTrack()
    {
        instance.audioSource.Play();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
