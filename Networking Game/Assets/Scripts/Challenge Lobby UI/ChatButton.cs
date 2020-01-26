using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

public class ChatButton : MonoBehaviour {
    
    public RpsAgent associatedPlayer;
    public ChallengeLobbyManager localChallengeLobbyManager;

	// Use this for initialization
	void Start () {
        localChallengeLobbyManager = GameObject.FindObjectOfType<ChallengeLobbyManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void clickButton()
    {
        //Telling the main server to create chat boxes for ourselves and the other player
        Debug.Log("Telling server to make chat boxes");

        uint opponentConnID = associatedPlayer.netId.Value;
        uint myConnID = localChallengeLobbyManager.clientPlayer.netId.Value;
        string connectionMessage = opponentConnID + "_" + myConnID;

        Debug.Log(connectionMessage);

        NetworkManager.singleton.client.Send(LobbyNetworkScript.createChatMSGType, new StringMessage(connectionMessage));
    }
}
