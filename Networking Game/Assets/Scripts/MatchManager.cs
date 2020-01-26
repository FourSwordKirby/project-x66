using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class MatchManager : NetworkBehaviour {

    public static MatchManager instance;

    public RpsMatch RpsMatchPrefab;

    private List<MatchRequest> requests;
    private List<RpsMatch> ongoingMatches;

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        requests = new List<MatchRequest>();
        ongoingMatches = new List<RpsMatch>();
    }


    [Server]
    public static void StartMatchRequest(RpsAgent initiator, RpsAgent victim)
    {
        if (initiator.stars <= 0)
        {
            Debug.Log("You lost and can't request matches");
            return;
        }

        if(initiator == victim)
        {
            Debug.LogWarning("Server: Player trying to request match with himself.");
            return;
        }

        if (victim.IsInMatch)
        {
            Debug.Log("Player is already in a match");
            return;
        }

        if(instance.requests.Exists(r => (r.initiator == initiator)))
        {
            Debug.LogWarning("Server: Initiator Player has already requested a match.");
            return;
        }

        if(victim.CardCount() == 0 || victim.stars <= 0)
        {
            Debug.LogWarning("Server: Victim has no cards to play with.");
            return;
        }

        Debug.Log("Server creating a match request. Initiator: " + initiator.AgentName + " Victim: " + victim.AgentName);
        MatchRequest matchRequest = new MatchRequest(initiator, victim);
        instance.requests.Add(matchRequest);

        // Tell otherPlayer that they have a match request waiting.
        victim.NotifyMatchRequest(initiator, matchRequest.timeRemaining);
    }

    [Server]
    public static void RespondToRequest(RpsAgent victim, RpsAgent initiator, bool accepted)
    {
        int requestIndex = instance.requests.FindIndex(r => r.victim == victim && r.initiator == initiator);
        if(requestIndex < 0)
        {
            Debug.Log("Server: " + victim.AgentName + " tried responding to a match that does not exist.");
            // TODO: Notify victim.
            return;
        }
        else
        {
            MatchRequest request = instance.requests[requestIndex];
            if (accepted)
            {
                // Spawn new match.
                Debug.Log("Server: Match created between " + initiator.AgentName + " and " + victim.AgentName);
                RpsMatch match = GameObject.Instantiate<RpsMatch>(instance.RpsMatchPrefab);
                match.Initialize(request.initiator, request.victim);
                match.transform.SetParent(instance.transform);
                instance.ongoingMatches.Add(match);
                NetworkServer.Spawn(match.gameObject);
            }
            else
            {
                // Tell initiator that the victim declined.
            }
            instance.requests.RemoveAt(requestIndex);
        }
    }

    [ServerCallback]
    void Update()
    {
        if (requests.Count > 0)
        {
            requests.ForEach(r => r.timeRemaining -= Time.deltaTime);
            requests.RemoveAll(r => {
                // Tell initiator that his request timed out.
                return r.timeRemaining <= 0.0f;
            });
        }
    }

    void OnDestroy()
    {
        instance = null;
    }

    public static void EndMatch(RpsMatch rpsMatch)
    {
        instance.ongoingMatches.Remove(rpsMatch);
    }

    public static RpsMatch FindMatchWithPlayer(RpsAgent player)
    {
        return instance.ongoingMatches.Find(match => match.playerA == player || match.playerB == player);
    }
}
