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
        FleeMultiple
    }

    public StartState startState = StartState.Wander;
    public GameObject target;
    public List<Enemy> group;
    public StateMachine<Enemy> Fsm;

    void Awake()
    {
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
            default:
                state = new WanderState(this, Fsm, 30.0f);
                break;
        }
        Fsm.InitialState(state);
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        this.Fsm.Execute();
	}

    void FixedUpdate()
    {
        this.Fsm.FixedExecute();
    }

    override public void ChangeState(State<Mobile> new_state)
    {
        this.state.Exit();
        this.state = new_state;
        this.state.Enter();
    }
}
