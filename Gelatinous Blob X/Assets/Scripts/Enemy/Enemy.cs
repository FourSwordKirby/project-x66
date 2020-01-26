using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : SecurityMember {

    private const float PATROL_PAUSE_TIME = 5.0f;

    public enum StartState
    {
        Wander,
        ReachGoal,
        Flock,
        Flee,
        FleeMultiple,
        FollowPath,
        SeekTargetWithPath,
        Patrol,
    }

    public StartState startState = StartState.Wander;
    public GameObject target;
    public List<Enemy> group;
    public StateMachine<Enemy> Fsm;
    public float speed;

    public SecuritySystem parentSystem;

    public Material searchLightMaterial;
    public Material alertLightMaterial;

    // Patrol variables
    public NavMeshAgent NavAgent { get; private set; }
    public PatrolPath patrolPath;
    public float patrolPauseTime = PATROL_PAUSE_TIME;

    //Hacky stuff used to test if preferred path
    public bool onFire;

    // Dangerous
    private bool isDangerous;
    public GameObject dangerHitbox;

    private Bounds CreateBodyBounds()
    {
        Bounds b = new Bounds();
        foreach(Collider c in this.GetComponentsInChildren<Collider>())
        {
            b.Encapsulate(c.bounds);
        }
        return b;
    }

    private Bounds _bodyBounds;
    public Bounds BodyBounds
    {
        get
        {
            if(_bodyBounds == null)
            {
                _bodyBounds = CreateBodyBounds();
            }
            return _bodyBounds;
        }
    }

    //TODO: is this implementation sufficient? Currently things aren't cones
    public GameObject targetingCone;

    void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        NavAgent.speed = speed;
        NavAgent.stoppingDistance = 0.2f;
    }

	// Use this for initialization
	void Start () 
    {
        // Self references
        this.dangerHitbox = this.transform.FindChild("Danger Zone").gameObject;
        this.targetingCone = this.transform.FindChild("Line of Sight").gameObject;
        _bodyBounds = CreateBodyBounds();
        Fsm = new StateMachine<Enemy>(this);
        State<Enemy> state;
        switch (this.startState)
        {
            case StartState.Wander:
                state = new WanderState(this, Fsm, 30.0f);
                break;
            case StartState.ReachGoal:
                state = new ReachGoalState(this, Fsm, target);
                break;
            case StartState.Flock:
                state = new FlockState(this, Fsm, target, group);
                break;
            case StartState.Flee:
                state = new FleeState(this, Fsm, target);
                break;
            case StartState.FleeMultiple:
                state = new FleeMultipleState(this, Fsm, group);
                break;
            case StartState.FollowPath:
                Debug.Log("Setting state");
                state = new FollowPathState(this, Fsm, PathFinding.spawnGameObjectsAtPathPoints(PathFinding.generatePath(gameObject, target)));
                Debug.Log("Done setting state");
                break;
            case StartState.SeekTargetWithPath:
                state = new SeekTargetWithPathState(this, Fsm, target);
                break;
            case StartState.Patrol:
                state = new PatrolState(this, Fsm, patrolPath, patrolPauseTime);
                break;
            default:
                state = new WanderState(this, Fsm, 30.0f);
                break;
        }
        Fsm.InitialState(state);

        SetDangerous(false);
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(this.Fsm.CurrentState);
        this.Fsm.Execute();
	}

    void FixedUpdate()
    {
        this.Fsm.FixedExecute();
    }


    override public void Alert(GameObject target)
    {
        //Broken right now because we don't have a nav mesh
        SetDangerous(true);
        this.gameObject.GetComponentInChildren<MeshRenderer>().material = alertLightMaterial;
        this.Fsm.ChangeState(new SeekTargetWithPathState(this, this.Fsm, target));
        //this.Fsm.ChangeState(new ReachGoalState(this, this.Fsm, target));
    }

    override public void ReturnToPosition()
    {
        SetDangerous(false);
        this.gameObject.GetComponentInChildren<MeshRenderer>().material = searchLightMaterial;
        this.Fsm.ChangeState(new PatrolState(this, Fsm, patrolPath, patrolPauseTime));
    }

    override public SecurityMemberStatus GetStatus(GameObject target)
    {
        //Currently pretend we aren't engaging the enemy
        if (targetingCone.GetComponent<Collider>().bounds.Contains(target.transform.position))
            return SecurityMemberStatus.Engaged;
        else
            return SecurityMemberStatus.Unengaged;
    }

    void OnCollisionEnter(Collision col)
    {
        Player p;
        if ((p = col.gameObject.GetComponent<Player>()) != null)
        {
            Debug.Log("Touched a player");
            //Time to do something with entering a pseudo alerted state
            parentSystem.Alert(col.gameObject);
            //this.Fsm.ChangeState(new SeekTargetWithPathState(this, this.Fsm, p.gameObject));
        }
    }

    //TODO: FIX THIS
    void OnTriggerEnter(Collider col)
    {
        Player p;
        if ((p = col.gameObject.GetComponent<Player>()) != null)
        {
            Debug.Log("Seen a player");
            //Time to do something with entering a pseudo alerted state
            if(parentSystem != null)
                parentSystem.Alert(col.gameObject);
            //this.Fsm.ChangeState(new SeekTargetWithPathState(this, this.Fsm, p.gameObject));
        }
    }
    

    public void SetDangerous(bool danger)
    {
        this.isDangerous = danger;
        dangerHitbox.SetActive(danger);
    }
}
