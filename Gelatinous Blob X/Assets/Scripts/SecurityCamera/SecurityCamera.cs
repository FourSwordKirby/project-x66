using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecurityCamera:  SecurityMember, ElectricDevice
{
    public float turnSpeed;

    public StateMachine<SecurityCamera> Fsm;
    public List<GameObject> targetingPoints;
    public Vector2 xFacingRange;    //Holds how far the camera can turn along the x axis (up and down)
    public Vector2 yFacingRange;    //Holds how far the camera can turn along the y axis (left and right)
    public Vector2 zFacingRange;    //Holds how far the camera can turn along the y axis (left and right)

    public SecuritySystem parentSystem;

    public float alertThreshold;
    public float deactivationThreshold;
    public float disabledTime;
    public bool cycleTargets;


    public Material searchLightMaterial;
    public Material suspiciousLightMaterial;
    public Material alertLightMaterial;
    public Material disabledLightMaterial;

    public AudioClip alertSound;
    public AudioClip disableSound;

    //the rotation body is the part of the model that will be rotating
    public GameObject rotationBody;
    public GameObject targetingCone;
    private AudioSource audioSource;

	// Use this for initialization
    void Awake()
    {
        this.audioSource = this.GetComponent<AudioSource>();
        Fsm = new StateMachine<SecurityCamera>(this);
        Fsm.InitialState(new CameraIdleState(this, Fsm));
    }

	void Start () {
	    if(parentSystem == null)
        {
            parentSystem = this.transform.GetComponentInParent<SecuritySystem>();
        }
	}

	void Update () {
        //base.decrementCounter();
        //if (!isDisabled)
            this.Fsm.Execute();
	}

    void FixedUpdate()
    {
        //if (!isDisabled)
            this.Fsm.FixedExecute();
    }

    void OnTriggerEnter(Collider col)
    {
        //TODO: Need to replace checking name with better things
        if(col.gameObject.name == "Player Body")
        {
            Debug.Log("Seen a player");
            //Time to do something with entering a pseudo alerted state
            Fsm.ChangeState(new CameraSuspiciousState(this, Fsm, col.gameObject));
        }
    }

    override public void Alert(GameObject target)
    {
        Debug.Log("alerted");
    }

    override public void ReturnToPosition()
    {
        if(this.Fsm.CurrentState.ToString() != "CameraIdleState")
            this.Fsm.ChangeState(new CameraIdleState(this, this.Fsm));
    }

    override public SecurityMemberStatus GetStatus(GameObject target)
    {
        //Kind of jank right now, might just be better to ray cast quickly
        if (targetingCone.GetComponent<Collider>().bounds.Contains(target.transform.position))
            return SecurityMemberStatus.Engaged;
        else
            return SecurityMemberStatus.Unengaged;
    }

    public void Trigger()
    {
        AudioSource.PlayClipAtPoint(disableSound, transform.position);
        DisableCamera();
    }

    public void DisableCamera(float duration = 0.0f)
    {
        float time = duration <= 0.0f ? this.disabledTime : duration;
        this.Fsm.ChangeState(new CameraDisabledState(this, this.Fsm, time));
    }

    public void ForceEnableCamera()
    {
        this.Fsm.ChangeState(new CameraIdleState(this, this.Fsm));
    }

    public void PlayAlertSound()
    {
        if(audioSource.clip != alertSound)
        {
            audioSource.clip = alertSound;
            audioSource.Play();
        }
    }

    public void StopSound()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }
}
