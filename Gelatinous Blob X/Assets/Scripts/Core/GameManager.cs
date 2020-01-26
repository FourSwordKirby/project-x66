using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    private static GameManager instance;

    private static Player _player;
    public static Player Player
    {
        get
        {
            if (_player == null) FindPlayer();
            return _player;
        }
    }

    public static DialogBox dialogBox;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void Start()
    {
        FindPlayer();
        if(dialogBox == null)
        {
            dialogBox = GameObject.FindObjectOfType<DialogBox>(); //This is maybe kind of jank? Perhaps there should just be a HUD class?
            dialogBox.closeDialog();
        }
    }

    void OnLevelWasLoaded()
    {
        FindPlayer();
        if (dialogBox == null)
        {
            dialogBox = GameObject.FindObjectOfType<DialogBox>(); //This is maybe kind of jank? Perhaps there should just be a HUD class?
            dialogBox.closeDialog();
        }
    }

    public static void displayDialog(string name = "", string dialog = "", DisplaySpeed displaySpeed = DisplaySpeed.fast)
    {
        dialogBox.displayDialog(name, dialog, displaySpeed);
        if (name == "" && dialog == "")
            dialogBox.closeDialog();
    }

    public static void closeDialog()
    {
        dialogBox.closeDialog();
    }

    public static string getDialog()
    {
        return dialogBox.dialogField.text;
    }

    /*public static void LoadScene(string sceneName, bool persistPlayer = true)
    {
        if (persistPlayer)
            DontDestroyOnLoad(Player);
        else
        {
            Destroy(Player);
            _player = null;
        }

        Application.LoadLevel(sceneName);

        FindPlayer();
        FindCamera();
    }*/


    private static void FindPlayer()
    {
        _player = Object.FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.Log("Cannot find player on the current scene.");
        }
    }


    public static void PlayerDeath()
    {
        TransitionManager.Instance.FadeToDark(() => {
            Application.LoadLevel("GameOver");
        });
    }
}
