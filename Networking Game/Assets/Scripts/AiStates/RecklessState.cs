using UnityEngine;
using System.Collections;

public class RecklessState : AiState
{
    public RpsAI parentAI;

    //Fight any player, anywhere, to the best of their ability
    public RecklessState(RpsAI parentAI)
    {
        this.parentAI = parentAI;
    }

    override public bool decideOnPlayer(CardDecision decision, RpsAgent otherPlayer)
    {
        //Accept any match
        return true;
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
