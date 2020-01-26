using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MatchRequestPanel : MonoBehaviour {

    private RectTransform selfTransform;
    private static int instanceCount = 0;

    public RpsHumanPlayer localPlayer;
    public RpsAgent initiator;
    public Text matchRequestText;
    public Text timeoutText;
    public float timeout;

    public void Initialize(RpsHumanPlayer localPlayer, RpsAgent initiator, float timeout)
    {
        this.localPlayer = localPlayer;
        this.initiator = initiator;
        this.timeout = timeout;
    }

    public void Start()
    {
        selfTransform = this.GetComponent<RectTransform>();
        float delta = ((instanceCount % 5) - 2) * 0.02f;
        Vector2 deltaVector = new Vector2(delta, delta);
        selfTransform.anchorMin += deltaVector;
        selfTransform.anchorMax += deltaVector;
        instanceCount += 1;

        matchRequestText = this.transform.FindChild("RequestText").GetComponent<Text>();
        matchRequestText.text = "Player " + initiator.AgentName + " has requested a match with you.";
        timeoutText = this.transform.FindChild("TimeoutText").GetComponent<Text>();
        UpdateTimeoutText();
    }
    	
    
    void Update()
    {
        // If localPlayer is in match, kill this panel and decline the match.
        if(localPlayer.IsInMatch)
        {
            OnDeclineButtonClick();
            Destroy(this.gameObject);
        }

        timeout -= Time.deltaTime;
        UpdateTimeoutText();
        if(timeout <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }

    public void UpdateTimeoutText()
    {
        timeoutText.text = Mathf.CeilToInt(timeout).ToString();
    }

    public void OnAcceptButtonClick()
    {
        localPlayer.CmdRespondToMatchRequest(initiator.netId, true);
        Destroy(this.gameObject);
    }

    public void OnDeclineButtonClick()
    {
        localPlayer.CmdRespondToMatchRequest(initiator.netId, false);
        Destroy(this.gameObject);
    }
}
