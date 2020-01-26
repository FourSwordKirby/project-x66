using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Collections.Generic;

public class ChallengeLobbyManager : MonoBehaviour {
    //The player associated with this client
    public RpsHumanPlayer clientPlayer;

    public List<RpsAgent> players;

    public EndMatchPanel endMatchPanelPrefab;

    public bool matchEnded = false;

    public Dictionary<Card, int> getTotalCardCounts()
    {
        Dictionary<Card, int> cardCounts = new Dictionary<Card, int>();
        cardCounts.Add(Card.Rock, 0);
        cardCounts.Add(Card.Paper, 0);
        cardCounts.Add(Card.Scissors, 0);

        foreach(RpsAgent player in players)
        {
            Dictionary<Card, int> playerCardCounts = player.getCardCounts();
            cardCounts[Card.Rock] += playerCardCounts[Card.Rock];
            cardCounts[Card.Paper] += playerCardCounts[Card.Paper];
            cardCounts[Card.Scissors] += playerCardCounts[Card.Scissors];
        }
        return cardCounts;
    }

    void Update()
    {
        //Write condition to end the game
        List<RpsAgent> activePlayers = players.FindAll(player => !(player.CardCount() == 0 || player.stars <= 0));
        Debug.Log(activePlayers.Count);
        if (activePlayers.Count <= 1 && !matchEnded && players.Count >= 2)
        {
            matchEnded = true;

            List<RpsAgent> winners = players.FindAll(player => (player.CardCount() == 0 && player.stars >= 0));
            List<RpsAgent> losers = players.FindAll(player => (player.stars <= 0));

            EndMatchPanel panel = GameObject.Instantiate<EndMatchPanel>(endMatchPanelPrefab);
            panel.transform.SetParent(this.transform, false);
            string endingMessage = "Game ended! Here are the winners and losers \n";
            endingMessage += "[WINNERS]: ";
            foreach (RpsAgent winner in winners)
            {
                endingMessage += winner.AgentName + ", ";
            }

            endingMessage += "\n [LOSERS]: ";
            foreach (RpsAgent loser in losers)
            {
                endingMessage += loser.AgentName + ", ";
            }

            panel.SetText(endingMessage);

            panel.OnReturnButtonClick += disconnect;

            panel.transform.SetParent(GameObject.FindObjectOfType<MatchPlayerUI>().transform,false);
        }
    }
    public void disconnect()
    {
        LobbyNetworkScript LobbyNetwork = GameObject.FindObjectOfType<LobbyNetworkScript>();
        LobbyNetwork.StopHost();
    }

/*    void Start()
    {
        LobbyNetworkScript LobbyNetwork = GameObject.FindObjectOfType<LobbyNetworkScript>();
        players = new List<RpsAgent>(GameObject.FindObjectsOfType<RpsAgent>());

        List<Vector3> positions = generatePlayerPositions(players.Count);

        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log("setting a players position");
            players[i].transform.position = positions[i];
        }

        //this.players = new List<RpsHumanPlayer>(GameObject.FindObjectsOfType<RpsHumanPlayer>());
    }

    public List<Vector3> generatePlayerPositions(int playerCount)
    {
        List<Vector3> positions = new List<Vector3>(playerCount);

        positions.Add(new Vector3(-3, 3, 0));
        positions.Add(new Vector3(-0, 3, 0));
        positions.Add(new Vector3(3, 3, 0));

        return positions;
    }
 */
}
