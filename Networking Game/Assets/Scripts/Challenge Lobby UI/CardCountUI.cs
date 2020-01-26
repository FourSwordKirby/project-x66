using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardCountUI : MonoBehaviour {

    private Text rockCountLabel;
    private Text paperCountLabel;
    private Text scissorCountLabel;

    private float updateTimer = 0.0f;
    private const float UPDATE_COOLDOWN = 0.5f;

	// Use this for initialization
	void Start () {
        rockCountLabel = GameObject.Find("Rock Count").GetComponent<Text>();
        paperCountLabel = GameObject.Find("Paper Count").GetComponent<Text>();
        scissorCountLabel = GameObject.Find("Scissor Count").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0.0f)
        {
            Dictionary<Card, int> totalCardCounts = GameObject.FindObjectOfType<ChallengeLobbyManager>().getTotalCardCounts();

            rockCountLabel.text = totalCardCounts[Card.Rock].ToString();
            paperCountLabel.text = totalCardCounts[Card.Paper].ToString();
            scissorCountLabel.text = totalCardCounts[Card.Scissors].ToString();
            updateTimer = UPDATE_COOLDOWN;
        }
	}
}
