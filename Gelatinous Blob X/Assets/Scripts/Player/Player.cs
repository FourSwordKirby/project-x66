using UnityEngine;
using System.Collections;

public class Player : Mobile {

    public const int DEFAULT_MAX_HEALTH = 100;
    public const int DEFAULT_MAX_STAMINA = 100;
    public const float STARTING_SPEED = 10;
    public const float INVULNERABLE_TIMER = 2.0f;
    public const float TRANSFORM_TO_CURRENT_COST = 20.0f;
    public const float STAMINA_RECOVERY_PER_SECOND = 5.0f;

    public float health
    {
        get;
        private set;
    }
    public float maxHealth
    {
        get;
        private set;
    }
    private bool isAlive;
    private bool isInvulnerable;
    private float invulnerableTimer;

    public float stamina { get; private set; }
    public float maxStamina { get; private set; }
    private bool canRecoverStamina;

    public float movementSpeed { get; set; } 

    public StateMachine<Player> ActionFsm { get; private set; }            //Populated with the states that the player can be in
    public State<Player> CurrentState { get { return ActionFsm.CurrentState; } }
    public Parameters.PlayerStatus status{get; private set;}     //Tells us the status of the player (things that affect the hitbox)

    // Interact Button related
    private bool canInteract;
    public Interactable CurrentInteractable
    {
        get;
        private set;
    }
    public bool IsTouchingInteractable
    {
        get
        {
            return CurrentInteractable != null;
        }
    }

    // Self references
    private Collider selfCollider;
    private Animator anim;
    private GameObject bodyVisual;
    private GameObject electricCurrentVisual;
    private GameObject deathVisual;
    public PlayerSounds Sounds { get; private set; }
    
    //Used for the initialization of internal, non-object variables
    void Awake()
    {
        maxHealth = DEFAULT_MAX_HEALTH;
        maxStamina = DEFAULT_MAX_STAMINA;
        movementSpeed = STARTING_SPEED;

        health = maxHealth;
        isAlive = true;
        stamina = maxStamina;
        this.SetStaminaRecovery(true);
        status = Parameters.PlayerStatus.Default;

        canInteract = true;
        CurrentInteractable = null;

        ActionFsm = new StateMachine<Player>(this);
        State<Player> startState = new IdleState(this, this.ActionFsm);
        ActionFsm.InitialState(startState);

        anim = this.GetComponent<Animator>();
        selfCollider = this.GetComponentInChildren<Collider>();
        bodyVisual = this.transform.FindChild("Billboard").gameObject;
        electricCurrentVisual = this.transform.FindChild("Electric Current Visual").gameObject;
        deathVisual = this.transform.FindChild("Death Animation").gameObject;

        Sounds = GetComponent<PlayerSounds>();
    }

	// Use this for initialization of variables that rely on other objects
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!isAlive)
        {
            return;
        }

        if(this.health <= 0.0f && isAlive)
        {
            isAlive = false;
            StartCoroutine(Die());
            return;
        }

        anim.SetBool("Damaged", isInvulnerable);
        if(isInvulnerable)
        {
            invulnerableTimer -= Time.deltaTime;
            if(invulnerableTimer <= 0.0f)
            {
                isInvulnerable = false;
            }
        }

        if(canRecoverStamina && this.stamina < this.maxStamina)
        {
            this.stamina += STAMINA_RECOVERY_PER_SECOND * Time.deltaTime;
            this.stamina = Mathf.Clamp(this.stamina, 0.0f, this.maxStamina);
        }

        if(this.canInteract && this.IsTouchingInteractable && Controls.interactInputDown())
        {
            CurrentInteractable.OnInteract(this);
        }

        //Controller support works sort of yaaaaay
        //Debug.Log(GamepadInput.GamePad.GetAxis(GamepadInput.GamePad.Axis.LeftStick, GamepadInput.GamePad.Index.One).x);
        this.ActionFsm.Execute();
	}

    private IEnumerator Die()
    {
        this.ActionFsm.ChangeState(new IdleState(this, this.ActionFsm));
        DisableColliders();
        Rigidbody body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.velocity = Vector3.zero;
        bodyVisual.SetActive(false);
        electricCurrentVisual.SetActive(false);
        deathVisual.SetActive(true);
        deathVisual.GetComponent<Animator>().SetTrigger("Die");
        yield return new WaitForSeconds(4.0f);
        GameManager.PlayerDeath();
    }

    void FixedUpdate()
    {
        this.ActionFsm.FixedExecute();
    }

    void OnTriggerStay(Collider collider)
    {
        if (CurrentInteractable == null)
        {
            this.CurrentInteractable = collider.GetComponent<Interactable>();
            if(CurrentInteractable != null)
            {
                this.CurrentInteractable.OnPlayerEnter(this);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (CurrentInteractable != null)
        {
            if (this.CurrentInteractable == collider.GetComponent<Interactable>())
            {
                this.CurrentInteractable.OnPlayerExit(this);
                this.CurrentInteractable = null;
            }
        }
    }

    void OnDisable()
    {
        this.ActionFsm.ChangeState(new IdleState(this, this.ActionFsm));
    }

    public void EnableIntearactions()
    {
        canInteract = true;
    }

    public void DisableInteractions()
    {
        canInteract = false;
    }

    public void EnableColliders()
    {
        EnableIntearactions();
        selfCollider.enabled = true;
    }

    public void DisableColliders()
    {
        DisableInteractions();
        selfCollider.enabled = false;

        // Disabling the colliders will skip the OnTriggerExit event
        // This becomes a problem since we only remove the reference
        // to the current interactable if we exit its trigger.
        CurrentInteractable = null;
    }

    public void SetLightningBody(bool visible)
    {
        electricCurrentVisual.SetActive(visible);
    }

    public bool CanTurnToElectricCurrent()
    {
        return this.stamina >= TRANSFORM_TO_CURRENT_COST;
    }

    public void TurnToElectricCurrent(ElectricNavpoint entryPoint)
    {
        this.UseStamina(TRANSFORM_TO_CURRENT_COST);
        this.ActionFsm.ChangeState(new ElectricState(this, this.ActionFsm, entryPoint));
    }

    //Need to code
    public void onHit()
    {
    }

    public void TakeDamage(int damage)
    {
        if(!isInvulnerable)
        {
            this.health -= damage;
            isInvulnerable = true;
            invulnerableTimer = INVULNERABLE_TIMER;
        }
    }

    public void GainStamina(float gain)
    {
        this.stamina += gain;
        this.stamina = Mathf.Clamp(this.stamina, 0.0f, this.maxStamina);
    }

    public void UseStamina(float cost)
    {
        this.stamina -= cost;
        this.stamina = Mathf.Clamp(this.stamina, 0.0f, this.maxStamina);
    }

    public void SetStaminaRecovery(bool active)
    {
        canRecoverStamina = active;
    }
}
