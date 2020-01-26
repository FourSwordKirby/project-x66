using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManager))]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class GameLobbyUI : MonoBehaviour {
    public LobbyNetworkScript manager;
    [SerializeField]
    public bool showGUI = true;
    [SerializeField]
    public int offsetX;
    [SerializeField]
    public int offsetY;

    public string playerName;


    // Runtime variable
    bool showServer = false;

    void Awake()
    {
        playerName = "Enter player name";
        manager = GetComponent<LobbyNetworkScript>();
    }

    void Update()
    {
        //if (!showGUI)
        //    return;

        //if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
        //{
        //    if (Input.GetKeyDown(KeyCode.H))
        //    {
        //        manager.StartHost();
        //    }
        //    if (Input.GetKeyDown(KeyCode.C))
        //    {
        //        manager.StartClient();
        //    }
        //}
        //if (NetworkServer.active && NetworkClient.active)
        //{
        //    if (Input.GetKeyDown(KeyCode.X))
        //    {
        //        manager.StopHost();
        //    }
        //}
    }

    void OnGUI()
    {
        if (!showGUI)
            return;

        int xpos = 10 + offsetX;
        int ypos = 40 + offsetY;
        int spacing = 24;

        //if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
        //{
        //    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "LAN Host(H)"))
        //    {
        //        manager.StartHost();
        //    }
        //    if (GUI.Button(new Rect(xpos + 200, ypos - 20, 120, 20), "Add AI player"))
        //    {
        //        manager.aiPlayerCount++;
        //    }
        //    manager.aiPlayerCount = int.Parse(GUI.TextArea(new Rect(xpos + 200, ypos, 120, 20), manager.aiPlayerCount.ToString()));
        //    if (GUI.Button(new Rect(xpos + 200, ypos + 20, 120, 20), "Remove AI player"))
        //    {
        //        if(manager.aiPlayerCount > 0)
        //            manager.aiPlayerCount--;
        //    }
        //    ypos += spacing;

        //    if (GUI.Button(new Rect(xpos, ypos, 105, 20), "LAN Client(C)"))
        //    {
        //        manager.StartClient();
        //    }
        //    manager.networkAddress = GUI.TextField(new Rect(xpos + 100, ypos, 95, 20), manager.networkAddress);
        //    ypos += spacing;

        //    //manager.lobbyPlayerPrefab.name =
        //    //    GUI.TextField(new Rect(xpos + 200, ypos + 40, 200, 40), manager.lobbyPlayerPrefab.name);
        //    this.playerName =
        //        GUI.TextField(new Rect(xpos + 200, ypos + 40, 200, 40), this.playerName);

        //}
        //else
        //{
        //    if (NetworkServer.active)
        //    {
        //        GUI.Label(new Rect(xpos, ypos, 300, 20), "Server: port=" + manager.networkPort);
        //        ypos += spacing;
        //    }
        //    if (NetworkClient.active)
        //    {
        //        GUI.Label(new Rect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
        //        ypos += spacing;
        //    }
        //}

        if (NetworkClient.active && !ClientScene.ready)
        {
            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Client Ready"))
            {
                ClientScene.Ready(manager.client.connection);

                if (ClientScene.localPlayers.Count == 0)
                {
                    ClientScene.AddPlayer(0);
                }
            }
            ypos += spacing;
        }

        if (NetworkServer.active || NetworkClient.active)
        {
            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Stop (X)"))
            {
                manager.StopHost();
            }
            ypos += spacing;
        }
        //Note if one ever needs to implement internet matchmaking, re-consult the NetworkManagerHUD script
    }
}
