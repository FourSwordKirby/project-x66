using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour {

    private LobbyNetworkScript Manager;
    private GameObject InitialMenu;
    private InputField NameInputField;
    private InputField AICountInputField;
    private InputField CardCopyInputField;
    private InputField JoinAddressInputField;
    private Text ServerInfoText;
    private Text ClientInfoText;
    private GameObject ReadyMenu;
    private Toggle ReadyCheck;
    private NetworkLobbyPlayer myPlayer;

    private string[] randomNames = { "Kaiji Itou",
                                       "Dio Brando", "Joseph Joestar", "Jonathan Joestar", "Jotaro Kujo",
                                   "Speedwagon", "Caesar", "Zeppeli", "Lisa Lisa", "Cars", "ACDC", "Whamu"};

    void Start()
    {
        Manager = FindObjectOfType<LobbyNetworkScript>();
        InitialMenu = this.transform.FindChild("InitialMenu").gameObject;
        NameInputField = InitialMenu.transform.FindChild("Name").GetComponent<InputField>();
        NameInputField.text = randomNames[Random.Range(0, randomNames.Length)];
        AICountInputField = InitialMenu.transform.FindChild("AI Count").GetComponent<InputField>();
        CardCopyInputField = InitialMenu.transform.FindChild("Copy Count").GetComponent<InputField>();
        JoinAddressInputField = InitialMenu.transform.FindChild("JoinAddress").GetComponent<InputField>();
        ServerInfoText = this.transform.FindChild("ServerInfo").GetComponent<Text>();
        ClientInfoText = this.transform.FindChild("ClientInfo").GetComponent<Text>();
        ReadyMenu = this.transform.FindChild("ReadyMenu").gameObject;
        ReadyCheck = ReadyMenu.transform.FindChild("ReadyCheck").GetComponent<Toggle>();
        myPlayer = null;
    }

    void Update()
    {
        if (!NetworkClient.active && !NetworkServer.active && Manager.matchMaker == null)
        {
            InitialMenu.SetActive(true);
            ReadyMenu.SetActive(false);
            ServerInfoText.gameObject.SetActive(false);
            ClientInfoText.gameObject.SetActive(false);
            AICountInputField.text = Manager.aiPlayerCount.ToString();
            CardCopyInputField.text = Manager.cardCopyCount.ToString();
            Manager.networkAddress = JoinAddressInputField.text;
            myPlayer = null;
        }
        else
        {
            InitialMenu.SetActive(false);
            ReadyMenu.SetActive(true);
            ServerInfoText.gameObject.SetActive(NetworkServer.active);
            if (NetworkServer.active)
            {
                ServerInfoText.text = "Server: port=" + Manager.networkPort;
            }

            ClientInfoText.gameObject.SetActive(NetworkClient.active);
            if (NetworkClient.active)
            {
                ClientInfoText.text = "Client: address=" + Manager.networkAddress + " port=" + Manager.networkPort;
            }
            if(myPlayer == null)
            {
                foreach(NetworkLobbyPlayer p in Manager.lobbySlots)
                {
                    if(p != null && p.isLocalPlayer)
                    {
                        myPlayer = p;
                    }
                }
            }
            else
            {
                ReadyCheck.isOn = myPlayer.readyToBegin;
            }
        }
    }

    public string GetPlayerName()
    {
        return NameInputField.text;
    }

    public void HostGameButtonClick()
    {
        Manager.StartHost();
    }

    public void AddAIButtonClick()
    {
        Manager.aiPlayerCount++;
    }

    public void SubAIButtonClick()
    {
        if(Manager.aiPlayerCount > 0)
        {
            Manager.aiPlayerCount--;
        }
    }

    public void AddCopyButtonClick()
    {
        Manager.cardCopyCount++;
    }

    public void SubCopyButtonClick()
    {
        if (Manager.cardCopyCount > 1)
        {
            Manager.cardCopyCount--;
        }
    }

    public void JoinGameButtonClick()
    {
        Manager.StartClient();
    }

    public void ReadyButtonClick()
    {
        if(myPlayer != null)
        {
            if (!ReadyCheck.isOn)
            {
                Debug.Log("Sending ready message!");
                myPlayer.SendReadyToBeginMessage();
            }
            else
            {
                myPlayer.SendNotReadyToBeginMessage();
            }
        }
    }

    //// Update is called once per frame
    //void Update () {

    //    if (NetworkClient.active && !ClientScene.ready)
    //    {
    //        if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Client Ready"))
    //        {
    //            ClientScene.Ready(Manager.client.connection);

    //            if (ClientScene.localPlayers.Count == 0)
    //            {
    //                ClientScene.AddPlayer(0);
    //            }
    //        }
    //        ypos += spacing;
    //    }

    //    if (NetworkServer.active || NetworkClient.active)
    //    {
    //        if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Stop (X)"))
    //        {
    //            Manager.StopHost();
    //        }
    //        ypos += spacing;
    //    }
    //}
}
