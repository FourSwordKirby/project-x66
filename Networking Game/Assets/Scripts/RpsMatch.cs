using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RpsMatch : NetworkBehaviour
{
    public enum State
    {
        WaitingForMove, Ready, TurnComplete
    }

    public RpsAgent playerA;
    public RpsAgent playerB;

    private int playerACardIndex;
    private int playerBCardIndex;

    private State _state;

    private float delay;
    private const float DELAY_TILL_REVEAL = 2.0f;

    public void Initialize(RpsAgent pA, RpsAgent pB)
    {
        playerA = pA;
        playerB = pB;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        playerA.BeginMatch(playerB);
        playerB.BeginMatch(playerA);
        _state = State.WaitingForMove;
    }

    [ServerCallback]
    void Update()
    {
        switch (_state)
        {
            case State.WaitingForMove:
                if (playerA.hasSelectedCard() && playerB.hasSelectedCard())
                {
                    _state = State.Ready;
                    delay = DELAY_TILL_REVEAL;
                    playerACardIndex = playerA.getSelectedCard(playerB);
                    playerBCardIndex = playerB.getSelectedCard(playerA);
                    string debugString = "Server: "
                        + string.Format("Player {0} picked {1} (index {2}). ", playerA.AgentName, playerA.CardAt(playerACardIndex), playerACardIndex)
                        + string.Format("Player {0} picked {1} (index {2}). ", playerB.AgentName, playerB.CardAt(playerBCardIndex), playerBCardIndex);
                    Debug.Log(debugString);
                }
                break;
            case State.Ready:
                delay -= Time.deltaTime;
                if(delay <= 0.0f)
                {
                    _state = State.TurnComplete;
                }
                break;
            case State.TurnComplete:
                PlayGameAndSendResults(playerACardIndex, playerBCardIndex);
                MatchManager.EndMatch(this);
                Destroy(this.gameObject);
                break;
        }
    }

    [Server]
    private void PlayGameAndSendResults(int a, int b)
    {
        Card cardA = playerA.CardAt(a);
        Card cardB = playerB.CardAt(b);

        playerA.RemoveCardAt(a);
        playerB.RemoveCardAt(b);

        playerA.AddToMoveSequence(cardA);
        playerB.AddToMoveSequence(cardB);

        if (cardA == cardB)
        {
            Debug.Log("Tie");
            playerA.EndMatch(MatchResult.Tie, cardB);
            playerB.EndMatch(MatchResult.Tie, cardA);
        }
        else if ((cardA == Card.Rock && cardB == Card.Scissors) ||
            (cardA == Card.Paper && cardB == Card.Rock) ||
            (cardA == Card.Scissors && cardB == Card.Paper)
            )
        {
            Debug.Log("Server: Player " + playerA.AgentName + " wins");
            playerA.EndMatch(MatchResult.Win, cardB);
            playerB.EndMatch(MatchResult.Lose, cardA);
        }
        else if ((cardB == Card.Rock && cardA == Card.Scissors) ||
            (cardB == Card.Paper && cardA == Card.Rock) ||
            (cardB == Card.Scissors && cardA == Card.Paper)
            )
        {
            Debug.Log("Server: Player " + playerB.AgentName + " wins");
            playerA.EndMatch(MatchResult.Lose, cardB);
            playerB.EndMatch(MatchResult.Win, cardA);
        }
    }
}
