using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RpsHumanPlayer : RpsAgent {

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        GameObject.FindObjectOfType<ChallengeLobbyManager>().clientPlayer = this;
    }

    public override void BeginMatch(RpsAgent opponent)
    {
        base.BeginMatch(opponent);
        RpcNotifyMatchStart(opponent.gameObject);
    }

    public delegate void MatchStartEvent(RpsAgent opponent);
    public event MatchStartEvent OnMatchStart;

    [ClientRpc]
    public void RpcNotifyMatchStart(GameObject opponentObj)
    {
        if(OnMatchStart != null)
        {
            OnMatchStart(opponentObj.GetComponent<RpsAgent>());
        }
    }

    public override void EndMatch(MatchResult result, Card opponentsCard)
    {
        base.EndMatch(result, opponentsCard);
        RpcNotifyMatchResult(result, opponentsCard);
    }

    public delegate void MatchEndEvent(MatchResult result, Card opponentsCard);
    public event MatchEndEvent OnMatchEnd;

    [ClientRpc]
    public void RpcNotifyMatchResult(MatchResult result, Card opponentsCard)
    {
        if (OnMatchEnd != null)
        {
            OnMatchEnd(result, opponentsCard);
        }
    }

    public override void NotifyMatchRequest(RpsAgent initiator, float timeout)
    {
        base.NotifyMatchRequest(initiator, timeout);
        RpcNotifyMatchRequest(initiator.gameObject, timeout);
    }

    public delegate void MatchRequestEvent(RpsAgent initiator, float timeout);
    public event MatchRequestEvent OnMatchRequest;

    [ClientRpc]
    public void RpcNotifyMatchRequest(GameObject initiatorObj, float timeout)
    {
        if (OnMatchRequest != null)
        {
            RpsAgent initator = initiatorObj.GetComponent<RpsAgent>();
            OnMatchRequest(initator, timeout);
        }
    }


    public override void NotifyMatchRequestResponse(RpsAgent victim, bool accepted)
    {
        base.NotifyMatchRequestResponse(victim, accepted);
        RpcNotifyMatchRequestResponse(victim.gameObject, accepted);
    }

    public delegate void MatchRequestResponseEvent(RpsAgent victim, bool accepted);
    public event MatchRequestResponseEvent OnMatchRequestResponse;

    [ClientRpc]
    public void RpcNotifyMatchRequestResponse(GameObject victimObj, bool matchAcepted)
    {
        if(OnMatchRequestResponse != null)
        {
            OnMatchRequestResponse(victimObj.GetComponent<RpsAgent>(), matchAcepted);
        }
    }


    [Command]
    public void CmdAddCard(int cardType)
    {
        AddCard((Card)cardType);
    }

    [Command]
    public void CmdRequestMatch(NetworkInstanceId victimID)
    {
        GameObject obj = NetworkServer.FindLocalObject(victimID);
        if (obj == null)
        {
            Debug.Log("Player requested match with non-existing object.");
            return;
        }

        RpsAgent victim = obj.GetComponent<RpsAgent>();
        if (victim == null)
        {
            Debug.Log("Player requested match with non-player object.");
            return;
        }
        RequestMatch(victim);
    }

    [Command]
    public void CmdSelectCard(int cardIndex)
    {
        hasSelectedCardForMatch = true;
        selectedCard = cardIndex;
    }

    [Command]
    public void CmdRespondToMatchRequest(NetworkInstanceId initiatorID, bool accepted)
    {
        GameObject obj = NetworkServer.FindLocalObject(initiatorID);
        if (obj == null)
        {
            Debug.Log("Player requested match with non-existing object.");
            return;
        }

        RpsAgent initiator = obj.GetComponent<RpsAgent>();
        if (initiator == null)
        {
            Debug.Log("Player requested match with non-player object.");
            return;
        }
        RespondToMatchRequest(initiator, accepted);
    }

    [Command]
    public void CmdObserveMatch(GameObject playerToObserveObj)
    {
        RpsAgent p1 = playerToObserveObj.GetComponent<RpsAgent>();
        RpsMatch match = MatchManager.FindMatchWithPlayer(p1);
        if(match != null)
        {
            RpcNotifyObserveMatch(true, match.playerA.gameObject, match.playerB.gameObject);
        }
        else
        {
            RpcNotifyObserveMatch(false, null, null);
        }
    }

    public delegate void ObserveMatchEvent(bool existsMatch, RpsAgent p1, RpsAgent p2);
    public event ObserveMatchEvent OnObserveMatch;

    [ClientRpc]
    public void RpcNotifyObserveMatch(bool existsMatch, GameObject p1, GameObject p2)
    {
        if(OnObserveMatch != null)
        {
            OnObserveMatch(existsMatch, p1.GetComponent<RpsAgent>(), p2.GetComponent<RpsAgent>());
        }
    }



    public override int getSelectedCard(RpsAgent otherPlayer)
    {
        return selectedCard;
    }

    public override bool hasSelectedCard()
    {
        return hasSelectedCardForMatch;
    }

}
