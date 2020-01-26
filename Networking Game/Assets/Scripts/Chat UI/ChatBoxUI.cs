using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

public class ChatBoxUI : MonoBehaviour {
    public Text OpponentNameLabel;
    public Text chatLog;
    public InputField inputField;

    public string chatID;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void clickButton()
    {
        if (inputField.text == "")
        {
            return;
        }

        string message = chatID + ": " + "[" + GameObject.FindObjectOfType<ChallengeLobbyManager>().clientPlayer.AgentName + "]" + inputField.text;
        inputField.text = "";

        //Send the message to the main server to process
        Debug.Log("Sending one message to the server");
        NetworkManager.singleton.client.Send(LobbyNetworkScript.chatMSGType, new StringMessage(message));
    }

    public void displayMessage(string message)
    {
        chatLog.text += message + "\n";
    }
}
