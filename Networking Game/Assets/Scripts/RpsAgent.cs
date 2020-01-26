using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;

public class RpsAgent : NetworkBehaviour {

    private Card[] defaultHand = new[]{Card.Rock, Card.Paper, Card.Scissors};

    public static int INITIAL_STARS = 3;
    public static int DEFAULT_STAR_CHANGE = 1;

    [SyncVar]
    public string AgentName;

    [SyncVar]
    public int stars;

    [SerializeField]
    [SyncVar]
    private string cards = "";

    [SerializeField]
    [SyncVar]
    private string moveSequence = "";

    [SyncVar]
    public Vector3 challegeLobbyPosition;

    [SyncVar]
    public bool IsInMatch = false;

    [SyncVar]
    public bool hasSelectedCardForMatch = false;

    [SyncVar]
    public int selectedCard = -1;


    public override void OnStartServer()
    {
        base.OnStartServer();
        this.stars = INITIAL_STARS;
       
        int cardCopyCount = GameObject.FindObjectOfType<LobbyNetworkScript>().cardCopyCount;

        foreach (Card c in defaultHand) {
            for (int i = 0; i < cardCopyCount; i++)
            {
                AddCard(c);
            }
        }
    }

    [Server]
    public void AddCard(Card cardType) {
        cards += ((int)cardType);
    }

    [Server]
    public void RemoveCardAt(int index)
    {
        cards = cards.Remove(index, 1);
    }

    public Card CardAt(int index)
    {
        return (Card)Char.GetNumericValue(cards[index]);
    }

    public int CardCount()
    {
        return cards.Length;
    }

    public int IndexOfCardInHand(Card c)
    {
        return cards.IndexOf(((int)c).ToString());
    }

    [Server]
    public void AddToMoveSequence(Card c)
    {
        moveSequence += (int)c;
    }

    public List<Card> GetMoveSequence()
    {
        List<char> charList = new List<char>(moveSequence.ToCharArray());
        return charList.ConvertAll(c => (Card)(int)Char.GetNumericValue(c));
    }

    [Server]
    public void RequestMatch(RpsAgent victim)
    {
        MatchManager.StartMatchRequest(this, victim);
    }


    [Server]
    public virtual void NotifyMatchRequest(RpsAgent initiator, float timeout)
    {
        
    }


    [Server]
    public void RespondToMatchRequest(RpsAgent initiator, bool accepted)
    {
        MatchManager.RespondToRequest(this, initiator, accepted);
    }

    [Server]
    public virtual void NotifyMatchRequestResponse(RpsAgent victim, bool accepted)
    {

    }

    [Server]
    public virtual void BeginMatch(RpsAgent opponent)
    {
        this.IsInMatch = true;
        this.hasSelectedCardForMatch = false;
        this.selectedCard = -1;
    }

    [Server]
    public virtual void EndMatch(MatchResult result, Card opponentsCard)
    {
        this.IsInMatch = false;
        switch(result)
        {
            case MatchResult.Lose:
                stars -= DEFAULT_STAR_CHANGE;
                //Removesall your cards when you lose
                if (stars <= 0)
                {
                    while (this.CardCount() > 0)
                        this.RemoveCardAt(this.CardCount() - 1);
                }
                break;
            case MatchResult.Tie:
                break;
            case MatchResult.Win:
                stars += DEFAULT_STAR_CHANGE;
                break;
        }
    }

    //Initializeds the agent with the local lobby manager
    public override void OnStartClient()
    {
        base.OnStartClient();
        GameObject.FindObjectOfType<ChallengeLobbyManager>().players.Add(this);
    }

    //used to get the totals of each card type
    public Dictionary<Card, int> getCardCounts()
    {
        Dictionary<Card, int> cardCounts = new Dictionary<Card, int>();
        cardCounts.Add(Card.Rock, 0);
        cardCounts.Add(Card.Paper, 0);
        cardCounts.Add(Card.Scissors, 0);

        for(int i = 0; i < CardCount(); ++i)
        {
            Card card = CardAt(i);
            cardCounts[card]++;
        }

        return cardCounts;
    }

    [Server]
    public virtual bool hasSelectedCard()
    {
        throw new MissingReferenceException("hasSelectedCard not implemented??");
    }

    public virtual int getSelectedCard(RpsAgent otherPlayer)
    {
        throw new MissingReferenceException("getSelectedCard not implemented??");
    }

    [Server]
    public virtual void nextTurn()
    {
        throw new MissingReferenceException("nextTurn not implemented??");

    }
}
