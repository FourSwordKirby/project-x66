using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Mobile {

    public enum StartState
    {
        Wander,
        ReachGoal,
        Flock,
        Flee,
        FleeMultiple,
        FollowPath,
        SeekTargetWithPath,
    }

    public StartState startState = StartState.Wander;
    public GameObject target;
    public List<Enemy> group;
    public StateMachine<Enemy> Fsm;
    public float speed;

    //Hacky stuff used to test if preferred path
    public bool onFire;

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

    void Awake()
    {
        speed = 5.0f;
    }

	// Use this for initialization
	void Start () {
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
            default:
                state = new WanderState(this, Fsm, 30.0f);
                break;
        }
        Fsm.InitialState(state);
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
    /*
    override public void ChangeState(State<Mobile> new_state)
    {
        this.state.Exit();
        this.state = new_state;
        this.state.Enter();
    }
     * */
}
