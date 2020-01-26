using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BalanceState : AiState
{
    public RpsAI parentAI;

    //An AI who cunningly tries to keep his hand balanced to counter future threats.
    public BalanceState(RpsAI parentAI)
    {
        this.parentAI = parentAI;
    }

    override public bool decideOnPlayer(CardDecision decision, RpsAgent otherPlayer)
    {
        Card opponentsCard = decisionToCard(decision);

        //Only accept requests which can leave our hand balanced
        Dictionary<Card, int> cardCounts = parentAI.getCardCounts();
        if (parentAI.IndexOfCardInHand(counterCard(opponentsCard)) != -1) {
            return (cardCounts.Values.Aggregate(0, Mathf.Max) - cardCounts[counterCard(opponentsCard)] <=1);
        }
        else if (parentAI.IndexOfCardInHand(opponentsCard) != -1) {
            return (cardCounts.Values.Aggregate(0, Mathf.Max) - cardCounts[opponentsCard] <=1);
        }
        else{
            //If we can't beat the card, we'll challenge if our hand is sufficiently out of whack in terms of balance
            return (isProbable(decision) && (cardCounts.Values.Aggregate(0, Mathf.Max) - cardCounts.Values.Aggregate(cardCounts.Count, Mathf.Max)) > 1);
        }

    }

    override public Card decideOnCard(CardDecision decision, RpsAgent otherPlayer)
    {
        Card opponentsCard = decisionToCard(decision);
        Dictionary<Card, int> cardCounts = parentAI.getCardCounts();

        //The AI will counter the player or match the player depending on which is more out of balance
        if (parentAI.IndexOfCardInHand(counterCard(opponentsCard)) != -1 && (cardCounts.Values.Aggregate(0, Mathf.Max) - cardCounts.Values.Aggregate(cardCounts.Count, Mathf.Max)) > 1)
            return counterCard(opponentsCard);
        else if (cardCounts[opponentsCard] == cardCounts.Values.Aggregate(0, Mathf.Max))
            return opponentsCard;
        else if (isProbable(decision) && (cardCounts.Values.Aggregate(0, Mathf.Max) - cardCounts.Values.Aggregate(cardCounts.Count, Mathf.Max)) > 1)
        {
            //return the card we have the most of
            return cardCounts.First(entry => entry.Value == cardCounts.Values.Aggregate(0, Mathf.Max)).Key;
        }
        else
        {
            //Shouldn't happen, but if it does, return a random card
            return (Card)Random.Range(0, 2);
        }
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
