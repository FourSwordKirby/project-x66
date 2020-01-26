using UnityEngine;
using System.Collections;

public class SecurityDoor :  SecurityMember, ElectricDevice
{
    public Material lockedMaterial;
    public Material unlockedMaterial;

    public bool closed;
    public bool locked;

    private float startingPosition;

    private const float closingDistance = 0.2f;

    private AudioSource audioSource;

    void Awake()
    {
        if (this.locked)
            this.GetComponent<MeshRenderer>().material = this.lockedMaterial;
        else
            this.GetComponent<MeshRenderer>().material = this.unlockedMaterial;

        this.startingPosition = this.transform.position.y;
        this.audioSource = this.GetComponent<AudioSource>();
    }

    void Start()
    {

    }

	// Update is called once per frame
	void Update () {
	    //Eventually do some stuff where if the boolean for closed is false, keep the door closed etc.
        if (this.closed)
        {
            if (this.transform.position.y <= startingPosition)
                this.transform.position += new Vector3(0, closingDistance, 0);
        }
        else
        {
            if(this.transform.position.y >= -startingPosition-closingDistance/2)
                this.transform.position -= new Vector3(0, closingDistance, 0);
        }
	}

    public void Open(){
        Debug.Log(this.closed);
        Debug.Log(this.locked);
        if (!this.locked && closed)
        {
            closed = false;
            audioSource.Play();
            Debug.Log("1" + this.closed);
            Debug.Log("2" + this.locked);
        }
    }

    public void Close(){
        if(!closed)
        {
            closed = true;
            audioSource.Play();
        }
    }

    public void Lock()
    {
        closed = true;
        locked = true;

        this.GetComponent<MeshRenderer>().material = this.lockedMaterial;
    }

    public void Unlock()
    {
        locked = false;

        this.GetComponent<MeshRenderer>().material = this.unlockedMaterial;
    }

    override public void Alert(GameObject target)
    {
        Lock();
    }

    override public void ReturnToPosition()
    {
        Unlock();
    }

    //The door can't engage with the player at all, it has no means of seeing the player, since it's a door
    override public SecurityMemberStatus GetStatus(GameObject target)
    {
        return SecurityMemberStatus.Unengaged;
    }

    public void Trigger()
    {
        if (this.locked)
            Unlock();
        else
            Lock();
    }
}
