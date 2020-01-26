using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SecuritySystem : MonoBehaviour {
    //Do we want to make a new class of objects which can be "alerted" so to speak
    public List<SecurityMember> secuitySystemComponents;
    
    private GameObject target;
    private GameObject lastKnownPosition;

    public float alertLength;
    private float alertCounter;

    //public Dictionary<SecurityMember, float> securityScheduler;

    void Start()
    {
        lastKnownPosition = new GameObject("Last Known Position");
        lastKnownPosition.transform.SetParent(this.transform);
    }

    void Update()
    {
        if (alertCounter > 0)
        {
            if(this.target == null)
                alertCounter -= Time.deltaTime;
            if (alertCounter < 0)
            {
                //Do some global stuff to return everyone to their normal routine
                foreach (SecurityMember member in secuitySystemComponents)
                {
                    //Some how need to stagger these
                    float delay = Random.Range(0.0f, 0.02f);
                    member.ReturnToPosition();
                }
            }
        }
    }


    public void Alert(GameObject target)
    {
        this.target = target;
        this.alertCounter = alertLength;

        foreach(SecurityMember member in secuitySystemComponents)
        {
            //Some how need to stagger these
            float delay = Random.Range(0.0f, 0.02f);
            member.Alert(target);
        }
    }

    public void lostSightofTarget(Vector3 lastKnownPositionVector)
    {
        //Basically, if no one can see the Security System's target, then we say we can't actually find the target and head towards the last known position
        if(!secuitySystemComponents.Any(x => x.GetStatus(target) == SecurityMemberStatus.Engaged))
        {
            this.target = null;
            lastKnownPosition.transform.position = lastKnownPositionVector;
            foreach (SecurityMember member in secuitySystemComponents)
            {
                member.Alert(lastKnownPosition);
            }
        }
    }
}