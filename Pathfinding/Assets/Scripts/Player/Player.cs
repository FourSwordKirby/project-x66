using UnityEngine;
using System.Collections;

public class Player : Mobile {

    private int health;
    private int maxHealth;
    public float movementSpeed { get; private set; } 

    public StateMachine<Player> ActionFsm { get; private set; }            //Populated with the states that the player can be in
    public Parameters.PlayerStatus status{get; private set;}     //Tells us the status of the player (things that affect the hitbox)

    private const int STARTING_HEALTH = 3;
    private const float STARTING_SPEED = 16.0f;
    
    //Used for the initialization of internal, non-object variables
    void Awake()
    {
        maxHealth = STARTING_HEALTH;
        movementSpeed = STARTING_SPEED;

        health = maxHealth;
        status = Parameters.PlayerStatus.Default;

        ActionFsm = new StateMachine<Player>(this);
        State<Player> startState = new IdleState(this, this.ActionFsm);
        ActionFsm.InitialState(startState);
    }

	// Use this for initialization of variables that rely on other objects
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //Controller support works sort of yaaaaay
        //Debug.Log(GamepadInput.GamePad.GetAxis(GamepadInput.GamePad.Axis.LeftStick, GamepadInput.GamePad.Index.One).x);
        this.ActionFsm.Execute();
	}

    void FixedUpdate()
    {
        this.ActionFsm.FixedExecute();
    }

    void OnTriggerEnter(Collider trigger)
    {
        Debug.Log("hither");
        if (Controls.interactInputDown())
        {
            Debug.Log("Charging");

            if(trigger.gameObject.GetComponent<ElectricNavpoint>() != null)
                this.ActionFsm.ChangeState(new ElectricState(this, this.ActionFsm, trigger.gameObject.GetComponent<ElectricNavpoint>()));
        }
    }

    //Need to code
    public void onHit()
    {
    }
}
