using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MatchPlayerUI : MonoBehaviour {

    private RpsHumanPlayer player;

    private HumanHandUI humanHand;
    private HandUI northHand;
    private GameObject northHandLabels;
    private HandUI southHand;
    private GameObject southHandLabels;

    private Image tableBackdropImage;

    public MatchRequestPanel matchRequestPanelPrefab;
    public EndMatchPanel endMatchPanelPrefab;

    private bool isHandUIVisible;

    public int CurrentPlayerCard
    {
        get
        {
            return humanHand.CurrentCard;
        }
    }



    void Awake()
    {
        isHandUIVisible = false;
    }

	// Use this for initialization
	void Start ()
    {
        player = transform.parent.GetComponent<RpsHumanPlayer>();
        if(!player.isLocalPlayer)
        {
            this.gameObject.SetActive(false);
            //Destroy(this.gameObject);
            return;
        }
        player.OnMatchRequest += DisplayMatchRequest;
        player.OnMatchStart += DisplayHumanMatchUI;
        player.OnMatchEnd += DisplayMatchResults;
        //TODO:
        //player.OnMatchRequestResponse += something
        player.OnObserveMatch += DisplayObservationMatch;

        matchResultAnimation = -1.0f;

        HandUI[] hands = this.GetComponentsInChildren<HandUI>();
        foreach(HandUI h in hands)
        {
            if(h is HumanHandUI)
            {
                humanHand = h as HumanHandUI;
                humanHand.Initialize(player);
            }
            if (h.dir == Direction.North)
            {
                northHand = h;
            }
            else
            {
                southHand = h;
            }
        }
        humanHand.gameObject.SetActive(false);
        northHand.gameObject.SetActive(false);
        southHand.gameObject.SetActive(false);

        tableBackdropImage = this.transform.FindChild("TableBackdrop").GetComponent<Image>();
        northHandLabels = this.transform.FindChild("NorthInfo").gameObject;
        southHandLabels = this.transform.FindChild("SouthInfo").gameObject;
        SetMatchUIActive(false);
	}
	
	// Update is called once per frame
    // If the parent doesn't have local player authority, return immediately.
	void Update ()
    {
        if(matchResultAnimation > 0.0f)
        {
            if(Input.GetMouseButtonUp(0))
            {
                matchResultAnimation = 0.0f;
            }
            matchResultAnimation -= Time.deltaTime;
            if(matchResultAnimation <= 0.0f)
            {
                CreateResultScreen();
            }
        }

        if(isSpectating)
        {
            if(spectatingAfterEndTimer > 0.0f)
            {
                spectatingAfterEndTimer -= Time.deltaTime;
                if(spectatingAfterEndTimer <= 0.0f)
                {
                    EndMatchPanel panel = GameObject.Instantiate<EndMatchPanel>(endMatchPanelPrefab);
                    panel.transform.SetParent(this.transform, false);
                    panel.SetText("Return to lobby?");
                    panel.OnReturnButtonClick += ExitObserveMatch;
                    isSpectating = false;
                }
            }
            else if(northHand.player.hasSelectedCardForMatch && southHand.player.hasSelectedCardForMatch)
            {
                northHand.SetAndRevealCard(northHand.player.CardAt(northHand.player.selectedCard));
                southHand.SetAndRevealCard(southHand.player.CardAt(southHand.player.selectedCard));
                spectatingAfterEndTimer = REVEAL_ANIMATION_LEGNTH;
            }
        }
	}


    private bool isSpectating = false;
    private float spectatingAfterEndTimer = -1.0f;
    public void DisplayObservationMatch(bool existsMatch, RpsAgent p1, RpsAgent p2)
    {
        if(!existsMatch)
        {
            EndMatchPanel panel = GameObject.Instantiate<EndMatchPanel>(endMatchPanelPrefab);
            panel.transform.SetParent(this.transform, false);
            panel.SetText("Unable to observe match.");
        }
        else
        {
            PrepareNorthHand(p2);
            PrepareSouthHand(p1);
            SetMatchUIActive(true);
            isSpectating = true;
        }
    }

    public void DisplayHumanMatchUI(RpsAgent opponent)
    {
        ExitObserveMatch();
        EndMatchPanel remnantPanel = this.GetComponentInChildren<EndMatchPanel>();
        if(remnantPanel != null)
        {
            Destroy(remnantPanel.gameObject);
        }

        PrepareHumanHand();
        PrepareNorthHand(opponent);
        SetMatchUIActive(true);
    }

    private const float REVEAL_ANIMATION_LEGNTH = 3.5f;
    private float matchResultAnimation;
    private MatchResult lastMatchResult;
    public void DisplayMatchResults(MatchResult result, Card opponentsCard)
    {
        Debug.Log("Client: Displaying match results: " +result);
        lastMatchResult = result;
        northHand.SetAndRevealCard(opponentsCard);
        matchResultAnimation = REVEAL_ANIMATION_LEGNTH;
    }

    public void CreateResultScreen()
    {
        EndMatchPanel panel = GameObject.Instantiate<EndMatchPanel>(endMatchPanelPrefab);
        panel.transform.SetParent(this.transform, false);
        panel.OnReturnButtonClick += ExitResultScreen;
        switch(lastMatchResult)
        {
            case MatchResult.Lose:
                panel.SetText("You lost!\nYou have lost " + RpsAgent.DEFAULT_STAR_CHANGE + " stars...");
                break;

            case MatchResult.Tie:
                panel.SetText("You tied!\nYou get to keep your wagered stars.");
                break;

            case MatchResult.Win:
                panel.SetText("You won!\nYou have gained " + RpsAgent.DEFAULT_STAR_CHANGE + " stars.");
                break;
        }
    }

    public void ExitResultScreen()
    {
        humanHand.gameObject.SetActive(false);
        northHand.Hide();
        SetMatchUIActive(false);
        if(player.stars == 0)
        {
            EndMatchPanel panel = GameObject.Instantiate<EndMatchPanel>(endMatchPanelPrefab);
            panel.transform.SetParent(this.transform, false);
            panel.SetText("You have no stars left! Wait until the game ends...");
        }
        else if (player.stars > 0 && player.CardCount() == 0)
        {
            EndMatchPanel panel = GameObject.Instantiate<EndMatchPanel>(endMatchPanelPrefab);
            panel.transform.SetParent(this.transform, false);
            panel.SetText("You've won the game, wait until the end of collect your prize!");
        }
    }

    public void ExitObserveMatch()
    {
        northHand.Hide();
        southHand.Hide();
        SetMatchUIActive(false);
        isSpectating = false;
    }

    private void SetLabelText(GameObject label, RpsAgent player)
    {
        label.transform.FindChild("Name").GetComponent<Text>().text = player.AgentName;
        label.transform.FindChild("Stats").GetComponent<Text>().text =
            "Remaining Cards: " + player.CardCount() + "\n" +
            "Stars: " + player.stars;

    }

    private void SetMatchUIActive(bool active)
    {
        tableBackdropImage.gameObject.SetActive(active);
        northHandLabels.SetActive(active);
        southHandLabels.SetActive(active);
    }

    public void PrepareNorthHand(RpsAgent player)
    {
        northHand.LoadForMatch(player);
        SetLabelText(northHandLabels, player);
    }

    public void PrepareSouthHand(RpsAgent player)
    {
        southHand.LoadForMatch(player);
        SetLabelText(southHandLabels, player);
    }

    public void PrepareHumanHand()
    {
        humanHand.LoadForMatch(this.player);
        SetLabelText(southHandLabels, this.player);
    }

    public void HoverNextPlayerCard()
    {
        humanHand.IncrementCurrentCard();
    }

    public void HoverPreviousPlayerCard()
    {
        humanHand.DecrementCurrentCard();
    }

    public void SelectCurrentPlayerCard()
    {
        humanHand.PlaySelectionAnimation();
    }


    public void DisplayMatchRequest(RpsAgent initator, float timeout)
    {
        if(!player.isLocalPlayer)
        {
            return;
        }
        MatchRequestPanel panel = GameObject.Instantiate<MatchRequestPanel>(matchRequestPanelPrefab);
        panel.transform.SetParent(this.transform, false);
        panel.Initialize(player, initator, timeout);
    }
    
    public void PlaySelectionAnimation()
    {
        humanHand.PlaySelectionAnimation();
    }
}
