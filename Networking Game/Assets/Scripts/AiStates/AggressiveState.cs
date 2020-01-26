using UnityEngine;
using System.Collections;

public class AggressiveState : AiState
{
    public RpsAI parentAI;

    //An AI who tries to get rid of their cards as fast as possible without losing stars
    public AggressiveState(RpsAI parentAI)
    {
        this.parentAI = parentAI;
    }

    override public bool decideOnPlayer(CardDecision decision, RpsAgent otherPlayer)
    {
        Card opponentsCard = decisionToCard(decision);
        //Accept requests we know we can win or tie
        return (parentAI.IndexOfCardInHand(counterCard(opponentsCard)) != -1 || parentAI.IndexOfCardInHand(opponentsCard) != -1);
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
