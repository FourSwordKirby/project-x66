using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LobbyNetworkScript : NetworkLobbyManager
{
    public List<ChatBoxUI> chatControllers;
    private Dictionary<string, List<int>> chatConnections;
    public List<string> playerNames;

    public int aiPlayerCount;
    public int cardCopyCount;

    public GameObject ChatBox;
    public GameObject aiPlayer;

    public static short chatMSGType = 555;
    public static short createChatMSGType = 556;
    public static short createPlayerType = 557;


    // Use this for initialization
    void Start()
    {
        cardCopyCount = 4;

        ClientScene.RegisterPrefab(aiPlayer);
        ResetFields();
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        base.OnLobbyServerDisconnect(conn);
        ResetFields();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        ResetFields();
    }

    public void ResetFields()
    {
        chatControllers = new List<ChatBoxUI>();
        chatConnections = new Dictionary<String, List<int>>();
        aiPlayerCount = 0;
        playerNames = new List<string>();
        occupiedSpaces = new List<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private List<Collider2D> occupiedSpaces;

    // for users to apply settings from their lobby player object to their in-game player object
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
        RpsAgent agent = gamePlayer.GetComponent<RpsAgent>();
        //Set the players name
        agent.AgentName = playerNames[lobbyPlayer.GetComponent<NetworkLobbyPlayer>().slot];


        //Create the player's lobby sprite at randomized locations
        AssignChallengeLobbyPosition(agent);

        //Disable it's in match UI
        //gamePlayer.GetComponent<RpsHumanPlayer>().IsHandUIVisible = false;
        Debug.Log("player spawned");

        return true;
    }

    //Adding the AI
    public override void OnLobbyServerSceneChanged(string sceneName)
    {
        base.OnLobbyServerSceneChanged(sceneName);

        Debug.Log("Server: moving into scene " + sceneName + "(lobbySCene is : " + lobbyScene + ")");
        if(sceneName == lobbyScene)
        {
            ResetFields();
        }

        //Now to add all the AI players
        for (int i = 0; i < aiPlayerCount; i++)
        {
            GameObject aiPlayer = Instantiate(this.aiPlayer);
            RpsAgent agent = aiPlayer.GetComponent<RpsAI>();
            agent.AgentName = "Unit 0" + i;

            //Create the player's lobby sprite at randomized locations
            AssignChallengeLobbyPosition(agent);

            NetworkServer.Spawn(aiPlayer);
        }
    }

    private void AssignChallengeLobbyPosition(RpsAgent agent, int maxTries = 50)
    {
        Collider2D agentCollider = agent.GetComponentInChildren<Collider2D>();
        bool freeSpace = false;
        float bestMargin = -1.0f;
        Vector3 posWithBestMargin = agent.transform.position;
        for (int trial = 0; trial < maxTries && !freeSpace; trial++)
        {
            freeSpace = true;
            agent.challegeLobbyPosition = new Vector3(UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-3.0f, 3.0f), 0);
            agent.transform.position = agent.challegeLobbyPosition;
            float worstOverlap = 1000000.0f;
            foreach (Collider2D col in occupiedSpaces)
            {
                if (agentCollider.bounds.Intersects(col.bounds))
                {
                    freeSpace = false;
                    float dist = (agentCollider.bounds.center - col.bounds.center).sqrMagnitude;
                    if(dist < worstOverlap)
                    {
                        worstOverlap = dist;
                    }
                }
            }
            if(!freeSpace)
            {
                if(worstOverlap > bestMargin)
                {
                    bestMargin = worstOverlap;
                    posWithBestMargin = agent.transform.position;
                }
            }
        }
        if(!freeSpace)
        {
            Debug.LogWarning("Server: Could not find good space for " + agent.AgentName + " in lobby. " +
                "Using best spot " + posWithBestMargin + " with margin " + bestMargin + ".");
            agent.challegeLobbyPosition = posWithBestMargin;
        }
        occupiedSpaces.Add(agentCollider);
    }

    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        base.OnLobbyClientConnect(conn);
        Debug.Log(GameObject.FindObjectOfType<LobbyPanel>().GetPlayerName());
        NetworkManager.singleton.client.Send(createPlayerType, new StringMessage(GameObject.FindObjectOfType<LobbyPanel>().GetPlayerName()));
    }


    public override void OnStopServer()
    {
        base.OnStopServer();
        chatControllers = new List<ChatBoxUI>();
        chatConnections = new Dictionary<String, List<int>>();
        aiPlayerCount = 0;
        playerNames = new List<string>();
    }

    //Done to make sure that game states are consistent e-e
    public override void OnServerDisconnect(NetworkConnection conn)
    {
 	    base.OnServerDisconnect(conn);
        StopServer();
        //Display a thing saying that you have been disconnected
    }

    // Lets the client receive chat messages
    public override void OnStartClient(NetworkClient mClient)
    {
        base.OnStartClient(mClient);
        mClient.RegisterHandler(chatMSGType, OnClientChatMessage);
        mClient.RegisterHandler(createChatMSGType, OnClientCreateChatMessage);
    }

    // Lets the main server receive messages to send to the rest of the clients
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler(chatMSGType, OnServerChatMessage);
        NetworkServer.RegisterHandler(createChatMSGType, OnServerCreateChatMessage);
        NetworkServer.RegisterHandler(createPlayerType, OnServerAddPlayer);
    }
    // Server relays any messages sent to it to all other clients
    private void OnServerAddPlayer(NetworkMessage netMsg)
    {
        Debug.Log("Server received a player to add");
        //Read the message and figure out the chatID and then who to send the message to
        string playerName = netMsg.ReadMessage<StringMessage>().value;
        Debug.Log("name: " + playerName);
        playerNames.Add(playerName);
    }


    // Server relays any messages sent to it to all other clients
    private void OnServerChatMessage(NetworkMessage netMsg)
    {
        Debug.Log("Server received a chat message to relay");
        //Read the message and figure out the chatID and then who to send the message to
        string networkMessage = netMsg.ReadMessage<StringMessage>().value;
        string chatID = networkMessage.Substring(0, networkMessage.IndexOf(":"));

        foreach(int connID in chatConnections[chatID])
            NetworkServer.SendToClient(connID, LobbyNetworkScript.chatMSGType, new StringMessage(networkMessage));
    }

    // Clients display chat messages upon receiving them
    private void OnClientChatMessage(NetworkMessage netMsg)
    {
        Debug.Log("Client receieved a message relayed by the server");
        string networkMessage = netMsg.ReadMessage<StringMessage>().value;
        string chatID = networkMessage.Substring(0, networkMessage.IndexOf(":"));
        string message = networkMessage.Substring(networkMessage.IndexOf(":") + 1);

        foreach (ChatBoxUI chatController in chatControllers)
        {
            if(chatController.chatID == chatID)
                chatController.displayMessage(message);
        }
    }

    private void OnServerCreateChatMessage(NetworkMessage netMsg)
    {
        Debug.Log("Server was told to tell people to create a chat box");
        string connectionMessage= netMsg.ReadMessage<StringMessage>().value;

        string [] netIDs = connectionMessage.Split('_');
        List<int> chatIDConnections = new List<int>();
        foreach (string netID in netIDs)
        {
            NetworkInstanceId objectId;
            foreach (NetworkInstanceId id in NetworkServer.objects.Keys)
            {
                if (id.Value == uint.Parse(netID))
                    objectId = id;
            }

            int connId = NetworkServer.FindLocalObject(objectId).GetComponent<NetworkIdentity>().connectionToClient.connectionId;
            chatIDConnections.Add(connId);
        }

        chatIDConnections.Sort();
        string chatId = chatIDConnections.Aggregate("", (id, accum) => id + accum);

        //Don't create more chat boxes if this specific one already exists
        if (chatConnections.ContainsKey(chatId))
            return;

        foreach (int connId in chatIDConnections)
        {
            NetworkServer.SendToClient(connId, LobbyNetworkScript.createChatMSGType, new StringMessage(chatId));
        }

        //Gotta make my chat id
        chatConnections.Add(chatId, chatIDConnections);
    }

    private void OnClientCreateChatMessage(NetworkMessage netMsg)
    {
        Debug.Log("Client is opening a chat box as requested by the server");
        GameObject ChatBox = Instantiate(this.ChatBox);

        GameObject chatboxParent = GameObject.Find("LocalUI");
        if(chatboxParent == null)
        {
            chatboxParent = GameObject.Find("Canvas");
        }
        ChatBox.transform.SetParent(chatboxParent.transform);
        ChatBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        ChatBox.GetComponentInChildren<ChatBoxUI>().chatID = netMsg.ReadMessage<StringMessage>().value;
        //ChatBox.GetComponent<ChatBoxUI>().OpponentNameLabel.text = "Chat Box";

        chatControllers.Add(ChatBox.GetComponentInChildren<ChatBoxUI>());
    }
}
