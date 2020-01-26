using UnityEngine;
using System.Collections;

public class ConservativeState : AiState
{
    public RpsAI parentAI;

    //An AI who is trying to gain stars
    public ConservativeState(RpsAI parentAI)
    {
        this.parentAI = parentAI;
    }

    override public bool decideOnPlayer(CardDecision decision, RpsAgent otherPlayer)
    {
        Card opponentsCard = decisionToCard(decision);
        //Only accept requests we know we can win
        return (parentAI.IndexOfCardInHand(counterCard(opponentsCard)) != -1);
    }

    override public Card decideOnCard(CardDecision decision, RpsAgent otherPlayer)
    {
        Card opponentsCard = decisionToCard(decision);
        //The AI blindly decides to counter the player decision or match it if possible
        if(parentAI.IndexOfCardInHand(counterCard(opponentsCard)) != -1)
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
