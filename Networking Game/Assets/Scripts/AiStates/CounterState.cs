using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CounterState : AiState
{
    public RpsAI parentAI;

    //A tactical AI who adjusts what they expect from the opponent with the number of cards still in play
    public CounterState(RpsAI parentAI)
    {
        this.parentAI = parentAI;
    }

    override public bool decideOnPlayer(CardDecision decision, RpsAgent otherPlayer)
    {
        Card opponentsCard = decisionToCard(decision);
        Dictionary<Card, int> totalCardCounts = GameObject.FindObjectOfType<ChallengeLobbyManager>().getTotalCardCounts();

        Card highestCardInPlay = totalCardCounts.Keys.Aggregate(Card.Rock, (x, y) => totalCardCounts[x] > totalCardCounts[y] ? x : y);
        if (parentAI.IndexOfCardInHand(counterCard(opponentsCard)) != -1)
        {
            return highestCardInPlay == opponentsCard;
        }
        return highestCardInPlay != opponentsCard;
    }

    override public Card decideOnCard(CardDecision decision, RpsAgent otherPlayer)
    {
        //If we don't know what the opponent will play, assume they are playing the card with the highest number in play
        if (decision == CardDecision.Random)
        {
            Dictionary<Card, int> totalCardCounts = GameObject.FindObjectOfType<ChallengeLobbyManager>().getTotalCardCounts();
            Card highestCardInPlay = totalCardCounts.Keys.Aggregate(Card.Rock, (x, y) => totalCardCounts[x] > totalCardCounts[y] ? x : y);

            if (parentAI.IndexOfCardInHand(counterCard(highestCardInPlay)) != -1)
                return counterCard(highestCardInPlay);
            else
                return highestCardInPlay;
        }

        Card opponentsCard = decisionToCard(decision);

        //The AI will do their best to counter the opponent
        if (parentAI.IndexOfCardInHand(counterCard(opponentsCard)) != -1)
            return counterCard(opponentsCard);
        else
            return opponentsCard;
    }

    override public void Enter()
    {
    }

    override public void Execute()
    {
    }

    override public void Exit()
    {
    }

}
