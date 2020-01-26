using UnityEngine;
using System.Collections;

public abstract class AiState {
    //Returns whether or not we should play this player based on what we think the other player is going to play and our own hand
    public abstract bool decideOnPlayer(CardDecision card, RpsAgent otherPlayer);

    //Returns which card we should play based on what we think the other player is going to play and our own hand
    public abstract Card decideOnCard(CardDecision card, RpsAgent otherPlayer);

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();

    //Returns the card that this decision would have you play
    public Card decisionToCard(CardDecision decision)
    {
        if (decision == CardDecision.ProbableRock || decision == CardDecision.DefiniteRock)
            return Card.Rock;
        else if (decision == CardDecision.ProbablePaper || decision == CardDecision.DefinitePaper)
            return Card.Paper;
        else if (decision == CardDecision.ProbableScissors || decision == CardDecision.DefiniteScissors)
            return Card.Scissors;
        else
            return (Card)Random.Range(0, 2);
    }

    //Basic used to see if a decision is probable
    public bool isProbable(CardDecision decision)
    {
        return (decision == CardDecision.ProbableRock || decision == CardDecision.ProbablePaper || decision == CardDecision.ProbableScissors);
    }

    //Returns the card that counters the input card
    public Card counterCard(Card card)
    {
        switch (card)
        {
            case (Card.Rock):
                return Card.Paper;
            case (Card.Paper):
                return Card.Scissors;
            case (Card.Scissors):
                return Card.Rock;
        }
        return Card.Rock;
    }
}
