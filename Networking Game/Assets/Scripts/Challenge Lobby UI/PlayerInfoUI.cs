using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerInfoUI : MonoBehaviour {

    public Vector3 initialPosition;
    public Vector3 targetPosition;
    public float speed;

    private Text playerName;
    private Text cardsLabel;
    private Text starsLabel;

    private Button observeButton;
    private Button challengeButton;
    private Button chatButton;

    private RectTransform selfTransform;

    private RpsAgent player;
    private ChallengeLobbyManager localChallengeLobbyManager;

    private bool isInView;

	// Use this for initialization
	void Start () {
        selfTransform = GetComponent<RectTransform>();
        initialPosition = selfTransform.anchoredPosition;
        targetPosition = selfTransform.anchoredPosition;

        playerName = GameObject.Find("Name").GetComponent<Text>();
        cardsLabel = GameObject.Find("Cards Label").GetComponent<Text>();
        starsLabel = GameObject.Find("Stars Label").GetComponent<Text>();

        observeButton = GameObject.Find("ObserveButton").GetComponent<Button>();
        challengeButton = GameObject.Find("ChallengeButton").GetComponent<Button>(); ;
        chatButton = GameObject.Find("ChatButton").GetComponent<Button>(); ;

        localChallengeLobbyManager = GameObject.FindObjectOfType<ChallengeLobbyManager>();

        speed = 1000.0f;
        isInView = false;
	}
	
	// Update is called once per frame
	void Update () {
        // Used to have the UI component slide in when needed
        var step = speed * Time.deltaTime;
        selfTransform.anchoredPosition = Vector3.MoveTowards(selfTransform.anchoredPosition, targetPosition, step);

        //Don't do anything if we don't have a player toggled
        if(player == null)
        {
            return;
        }   
        //Keep the labels updated
        playerName.GetComponent<Text>().text = player.AgentName;
        cardsLabel.GetComponent<Text>().text = "Cards Count: " + player.CardCount();
        starsLabel.GetComponent<Text>().text = "Stars: " + player.stars.ToString();

        // If the player got sucked into a match without closing the panel
        // we'll close it for them.
        if(isInView && localChallengeLobbyManager.clientPlayer.IsInMatch)
        {
            hidePanel();
        }

        bool isInPosition = (selfTransform.anchoredPosition3D - targetPosition).sqrMagnitude < 0.01f;
        if (isInView && isInPosition && Input.GetMouseButtonUp(0) &&
             !RectTransformUtility.RectangleContainsScreenPoint(selfTransform, Input.mousePosition))
        {
            hidePanel();
        }
	}

    public void loadInfo(RpsAgent player, bool isAIPlayer)
    {
        //if(this.player == player)
        //{
        //    return;
        //}

        GetComponent<RectTransform>().anchoredPosition = initialPosition;
        targetPosition = GetComponent<RectTransform>().anchoredPosition + new Vector2(-300, 0);

        this.player = player;
        /*need to set the associated player in the future
        observeButton.GetComponent<ChatButton>().associatedPlayer = player;
         */
        chatButton.interactable = !isAIPlayer;
        challengeButton.interactable = !player.IsInMatch && player.stars > 0;
        observeButton.interactable = player.IsInMatch && player.stars > 0;

        chatButton.GetComponent<ChatButton>().associatedPlayer = player;
        isInView = true;

        //TODO: Maybe some cool stuff where the player's name hashes to a unique color scheme
        //this.GetComponent<Image>().color = new Color(player.name.);
    }

    public void hidePanel()
    {
        targetPosition = initialPosition;
        chatButton.interactable = false;
        challengeButton.interactable = false;
        observeButton.interactable = false;
        isInView = false;
    }

    public void ChallengeButtonClick()
    {
        //Telling the main server to create chat boxes for ourselves and the other player
        Debug.Log("Local player " + localChallengeLobbyManager.clientPlayer.AgentName + " requesting a match with " + player.AgentName);
        localChallengeLobbyManager.clientPlayer.CmdRequestMatch(player.netId);
        hidePanel();
    }

    public void ObserveButtonClick()
    {
        Debug.Log("Local player " + localChallengeLobbyManager.clientPlayer.AgentName + " trying to observe " + player.AgentName);
        localChallengeLobbyManager.clientPlayer.CmdObserveMatch(player.gameObject);
        hidePanel();
    }
}
