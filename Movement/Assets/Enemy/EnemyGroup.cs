using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGroup : Enemy {

    public enum Formation
    {
        V,
        Circle,
        Square,
        Triangle,
        Polygon
    }

    public Formation formation = Formation.V;

    public float spread = 0.0f;

    public List<Enemy> GroupMembers;
    private List<GameObject> GroupPositions;

    public int formationSideCount;


    public EnemyGroup(int size, int spread) {
        GroupMembers = new List<Enemy>(size);
        GroupPositions = new List<GameObject>(size);

        this.spread = spread;
    }

    void Awake()
    {
        Fsm = new StateMachine<Enemy>(this);
        State<Enemy> state;
        switch (this.startState)
        {
            case StartState.Wander:
                state = new DumbWanderState(this, Fsm, 30.0f);
                break;
            case StartState.ReachGoal:
                state = new ReachGoalState(this, Fsm, target);
                break;
            default:
                state = new DumbWanderState(this, Fsm, 30.0f);
                break;
        }
        Fsm.InitialState(state);

        GroupPositions = new List<GameObject>(GroupMembers.Count);
        switch (this.formation)
        {
            case Formation.V:
                generatePositions(Formations.GetVFormation(this.transform.position, spread, GroupMembers.Count));
                break;
            case Formation.Circle:
                generatePositions(Formations.GetCircleFormation(this.transform.position + spread*(new Vector3(1, 0, 0)), this.transform.position, new Vector3(0, 1, 0), GroupMembers.Count));
                break;
            case Formation.Square:
                generatePositions(Formations.GetPolygonFormation(this.transform.position + spread*(new Vector3(1, 0, 0)), this.transform.position, new Vector3(0, 1, 0), 4, GroupMembers.Count));
                break;
            case Formation.Triangle:
                generatePositions(Formations.GetPolygonFormation(this.transform.position + spread * (new Vector3(1, 0, 0)), this.transform.position, new Vector3(0, 1, 0), 3, GroupMembers.Count));
                break;
            case Formation.Polygon:
                if(formationSideCount > 3)
                    generatePositions(Formations.GetPolygonFormation(this.transform.position + spread * (new Vector3(1, 0, 0)), this.transform.position, new Vector3(0, 1, 0), formationSideCount, GroupMembers.Count));
                break;
        }

    }

	// Use this for initialization
    void Start()
    {
        assignPostions();
        
	}
	
	// Update is called once per frame
	void Update () {
        this.Fsm.Execute();
	}

    void FixedUpdate()
    {
        this.Fsm.FixedExecute();
        GetComponent<Rigidbody>().velocity *= 1.1f;
    }

    override public void ChangeState(State<Mobile> new_state)
    {
        this.state.Exit();
        this.state = new_state;
        this.state.Enter();
    }

    public void addMember(Enemy enemy)
    {
        GroupMembers.Add(enemy);
    }

    //Given a list of positions formed by the formation algorithm, 
    //sets the group game object positions to those generated positions
    //Also sets those objects as children of the current object
    public void generatePositions(List<Vector3> positions)
    {
        Debug.Log(positions.Count);
        foreach (Vector3 position in positions)
        {
            GameObject GroupPosition = new GameObject();
            GroupPosition.transform.parent = this.transform;
            GroupPosition.transform.localPosition = position;
            GroupPositions.Add(GroupPosition);
        }
    }

    //Assigns each game object a position to head towards based on distance from said position
    public void assignPostions()
    {
        List<Enemy> PotentialGroupMembers = new List<Enemy>(GroupMembers);
        foreach (GameObject position in GroupPositions)
        {
            float minDistance = float.MaxValue;
            Enemy consideredEnemy = null;
            foreach(Enemy enemy in PotentialGroupMembers)
            {
                float distance = Vector3.Distance(enemy.transform.position, position.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    consideredEnemy = enemy;
                }
            }
            consideredEnemy.Fsm.ChangeState(new ReachGoalState(consideredEnemy, consideredEnemy.Fsm, position, 0.2f));
            PotentialGroupMembers.Remove(consideredEnemy);
        }
    }
}
