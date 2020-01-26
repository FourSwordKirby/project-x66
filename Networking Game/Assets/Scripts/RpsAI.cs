using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RpsAI : RpsAgent {

    public int nGramAmount = 3;
    public int minNGramSamplesNeeded = 3;

    public RpsAgent requestInitiator;
    public float requestDecisionTimer = 0.0f;
    public bool requestDecision;

    public RpsAgent proposalVictim;
    public float proposalTimer = 0.0f;

    public const float SLOW_DECISION_TIME = .2f;
    public const float FAST_DECISION_TIME = .2f;

    public const float SLOW_PROPOSAL_TIME = 40.0f;
    public const float FAST_PROPOSAL_TIME = 20.0f;

    public const float MIN_TIME_TO_CHOICE = 0.2f;
    public const float MAX_TIME_TO_CHOICE = 4.0f;
    public float timeToChoice;

    public int patience; //Describes how tactical our player is 

    public enum StrategyState {
        Reckless,   //Accept any request
        Aggressive,   //Accept requests we know we can win or tie
        Conservative, //Only accept requests we know we can win
        Balanced,   //Tend to accept requests which keep our hand balanced
        Counter     //Tend to accept requests which allow us to counter the current trend of total cards
    }

    //public StrategyState currentStrategy = StrategyState.Reckless;

    public AiState currentStrategyState;

    void Start()
    {
        patience = 4;
        currentStrategyState = new BalanceState(this);
    }

    //TODO
    [ServerCallback]
    void Update() {
        if (requestDecisionTimer > 0) {
            requestDecisionTimer -= Time.deltaTime;
            if (requestDecisionTimer <= 0)
                base.RespondToMatchRequest(requestInitiator, requestDecision);
            return;
        }
        if (!IsInMatch) {
            if (proposalVictim == null)
            {
                proposalVictim = findVictim();
            }
            proposalTimer -= Time.deltaTime;
            if (proposalTimer <= 0) {
                //Since we couldn't find a victim, we update our states and decrease our patience
                if (proposalVictim == null)
                {
                    patience--;
                    //Updates for while we are idling
                    if (patience > 0 && stars > 3)
                        currentStrategyState = new BalanceState(this);
                    else if (patience > 0 && 3 >= stars && stars > 1)
                        currentStrategyState = new ConservativeState(this);
                    else if (patience < 0 && stars > 5)
                        currentStrategyState = new RecklessState(this);
                    else if (patience < 0 && 5 >= stars)
                        currentStrategyState = new AggressiveState(this);
                    return;
                }
                RequestMatch(proposalVictim);
                proposalTimer = FAST_PROPOSAL_TIME;
            }
            return;
        }
        else
        {
            if(timeToChoice > 0.0f)
            {
                hasSelectedCardForMatch = false;
                timeToChoice -= Time.deltaTime;
            }
            else
            {
                hasSelectedCardForMatch = true;
            }
        }
    }

    public override void BeginMatch(RpsAgent opponent) {
        base.BeginMatch(opponent);
        proposalVictim = null;
        timeToChoice = Random.Range(MIN_TIME_TO_CHOICE, MAX_TIME_TO_CHOICE);
    }

    public override void NotifyMatchRequest(RpsAgent initiator, float timeout) {
        //Instantly respond to the request because we're going to respond to someone else soon
        if (requestDecisionTimer > 0) {
            base.RespondToMatchRequest(initiator, false);
            return;
        }

        this.requestInitiator = initiator;
        requestDecision = makeRequestDecision(initiator);
        requestDecisionTimer = SLOW_DECISION_TIME;
    }

    public override void EndMatch(MatchResult result, Card opponentsCard)
    {
        base.EndMatch(result, opponentsCard);
        if (result == MatchResult.Lose)
            patience--;
        else
            patience++;

        //Updates after a loss
        if (patience > 0 && stars > 3)
            currentStrategyState = new BalanceState(this);
        else if (patience > 0 && 3 >= stars && stars > 1)
            currentStrategyState = new ConservativeState(this);
        else if (patience < 0 && stars > 5)
            currentStrategyState = new RecklessState(this);
        else if (patience < 0 && 5 >= stars)
            currentStrategyState = new AggressiveState(this);
        return;
    }

    public override int getSelectedCard(RpsAgent otherPlayer) {
        CardDecision opponentsDecision = getCardDecision(otherPlayer);

        Card bestCard = this.currentStrategyState.decideOnCard(opponentsDecision, otherPlayer);

        int index = IndexOfCardInHand(bestCard);
        if (index > -1)
        {
            selectedCard = index;
        }
        selectedCard = returnRandomCardFromHand();
        return selectedCard;
    }

    public override bool hasSelectedCard()
    {
        return hasSelectedCardForMatch;
    }


    //Iterates through the players finding one which is suitable to challenge
    private RpsAgent findVictim() {
        List<RpsAgent> players = GameObject.FindObjectOfType<ChallengeLobbyManager>().players;
        List<RpsAgent> viablePlayers = players.FindAll(player => currentStrategyState.decideOnPlayer(getCardDecision(player), player) && player.CardCount() > 0);

        //If there are no players that we can viably play against, we simply return null
        if (viablePlayers.Count == 0)
        {
            return null;
        }

        return viablePlayers[Random.Range(0, viablePlayers.Count)];
    }

    private bool makeRequestDecision(RpsAgent otherPlayer) {
        CardDecision opponentsDecision = getCardDecision(otherPlayer);
        return currentStrategyState.decideOnPlayer(opponentsDecision, otherPlayer);
    }

    private CardDecision getCardDecision(RpsAgent otherPlayer) {
        List<Card> otherMoves = otherPlayer.GetMoveSequence();
        CardTree cardTree = getCardTree(otherMoves);

        // Look up the most likely move based on N-gram tree
        // Tries to do N-gram first, if not enough trials, decreases N and tries again (until N == 1)
        for (int nGramN = nGramAmount; nGramAmount >= 1; nGramAmount--)
        {
            Queue<Card> lastNMoves = new Queue<Card>();
            for (int i = Mathf.Max(otherMoves.Count - nGramN, 0); i < otherMoves.Count; i++)
            {
                lastNMoves.Enqueue(otherMoves[i]);
            }

            CardTree cardTreeNode = cardTree;
            while (lastNMoves.Count > 0)
            {
                cardTreeNode = cardTreeNode.Traverse(lastNMoves.Dequeue());
            }

            if (cardTreeNode.totalCount < minNGramSamplesNeeded)
            {
                continue;
            }

            Card likelyCard = cardTreeNode.MostLikelyCard();
            Dictionary<Card, int> opponentCardCounts = otherPlayer.getCardCounts();

            if (likelyCard == Card.Rock)
            {
                if (opponentCardCounts[Card.Paper] == 0 && opponentCardCounts[Card.Scissors] == 0)
                    return CardDecision.DefiniteRock;
                return CardDecision.ProbableRock;
            }
            else if (likelyCard == Card.Paper)
            {
                if (opponentCardCounts[Card.Scissors] == 0 && opponentCardCounts[Card.Rock] == 0)
                    return CardDecision.DefinitePaper;
                return CardDecision.ProbablePaper;
            }
            else if (likelyCard == Card.Scissors)
            {
                if (opponentCardCounts[Card.Rock] == 0 && opponentCardCounts[Card.Paper] == 0)
                    return CardDecision.DefiniteScissors;
                return CardDecision.ProbableScissors;
            }
        }
        return CardDecision.Random;
    }

    //Returns a random card from our hand
    private int returnRandomCardFromHand() {
        return Random.Range(0, CardCount());
    }


    private CardTree getCardTree(List<Card> otherMoves)
    {
        /* Constructs a prefix tree for looking up N-grams, since the number of possible
         * N-move sequences is 3^N
         * 
         * The prefix tree actually stores all the information needed for 1 to N grams
         */

        CardTree cardTreeHead = new CardTree(-1);
        CardTree cardTreeNode;

        for (int i = 0; i < otherMoves.Count; i++)
        {
            cardTreeNode = cardTreeHead;

            for (int j = i; j < Mathf.Min(otherMoves.Count, i + nGramAmount + 1); j++)
            {
                cardTreeNode.Add(otherMoves[j]);
                cardTreeNode = cardTreeNode.Traverse(otherMoves[j]);
            }
        }

        return cardTreeHead;
    }

    private class CardTree {
        public int count;

        private int rockCount { get { if (rockTree != null) return rockTree.count; return 0; } }
        private int paperCount { get { if (paperTree != null) return paperTree.count; return 0; } }
        private int scissorCount { get { if (scissorsTree != null) return scissorsTree.count; return 0; } }

        public int totalCount { get { return rockCount + paperCount + scissorCount; } }

        public CardTree rockTree { get; private set; }
        public CardTree paperTree { get; private set; }
        public CardTree scissorsTree { get; private set; }

        public CardTree(int c) {
            count = c;
        }

        public void Add(Card c) {
            switch (c) {
                case Card.Rock:
                    if (rockTree == null) {
                        rockTree = new CardTree(1);
                    }
                    else {
                        rockTree.count++;
                    }
                    break;
                case Card.Paper:
                    if (paperTree == null) {
                        paperTree = new CardTree(1);
                    }
                    else {
                        paperTree.count++;
                    }
                    break;
                case Card.Scissors:
                    if (scissorsTree == null) {
                        scissorsTree = new CardTree(1);
                    }
                    else {
                        scissorsTree.count++;
                    }
                    break;
            }
        }

        public CardTree Traverse(Card c) {
            switch (c) {
                case (Card.Rock):
                    return rockTree;
                case (Card.Paper):
                    return paperTree;
                case (Card.Scissors):
                    return scissorsTree;
            }
            return null;
        }

        public Card MostLikelyCard() {
            if (rockCount >= scissorCount && rockCount >= paperCount) {
                return Card.Rock;
            }
            else if (paperCount >= scissorCount) {
                return Card.Paper;
            }
            return Card.Scissors;
        }
    }
}