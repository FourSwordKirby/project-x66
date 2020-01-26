using UnityEngine;
using UnityEngine.EventSystems;

public class HumanPlayerControls : MonoBehaviour {

    private RpsHumanPlayer player;
    private MatchPlayerUI view;

    private float delayTillSelection;
    private const float SELECTION_DELAY = 2.0f;

	// Use this for initialization
    void Start()
    {
        player = transform.parent.GetComponent<RpsHumanPlayer>();
        if (!player.isLocalPlayer)
        {
            this.gameObject.SetActive(false);
            //Destroy(this.gameObject);
            return;
        }
        view = player.GetComponentInChildren<MatchPlayerUI>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!player.isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Client: Alpha1 pressed.");
            player.CmdAddCard((int)Card.Rock);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Client: Alpha2 pressed.");
            player.CmdAddCard((int)Card.Paper);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Client: Alpha3 pressed.");
            player.CmdAddCard((int)Card.Scissors);
        }

        if(!player.IsInMatch)
        {
            return;
        }
        
        if(player.hasSelectedCard())
        {
            return;
        }
                
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            view.HoverPreviousPlayerCard();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            view.HoverNextPlayerCard();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            view.PlaySelectionAnimation();
            player.CmdSelectCard(view.CurrentPlayerCard);
            return;
        }
	}
}
