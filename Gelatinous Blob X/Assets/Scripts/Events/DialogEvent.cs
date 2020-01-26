using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This dialog script is used for in game cutscenes
//Player can't move while the rest of the world moves around them.
public class DialogEvent : MonoBehaviour {
    public TextAsset textFile;

    private string name;
    private string[] dialog;
    private int dialogCounter;

    private bool inDialog = false;

    string[] separatingStrings = { "\n" };

	// Use this for initialization
	void Start () {
        this.name = textFile.text.Substring(0, textFile.text.IndexOf('\n'));
        this.dialog = textFile.text.Substring(textFile.text.IndexOf('\n')+1).Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        this.dialogCounter = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (inDialog)
        {
            if (Controls.interactInputDown())
            {
                //Displays the dialog immediately if in progress
                if (GameManager.getDialog() != dialog[dialogCounter - 1])
                {
                    GameManager.displayDialog(name, dialog[dialogCounter - 1], DisplaySpeed.immediate);
                    return;
                }

                if (dialogCounter == dialog.Length)
                {
                    this.inDialog = false;
                    GameManager.closeDialog();

                    //Restore player control
                    GameManager.Player.enabled = true;

                    //This is a one time event destroy this trigger
                    Destroy(this.gameObject);
                }
                else{
                    GameManager.displayDialog(name, dialog[dialogCounter]);
                    dialogCounter++;
                }
            }
        }
	}

	void OnTriggerEnter(Collider col)
	{
        if(col.gameObject.GetComponent<Player>() != null)
        {
            inDialog = true;
            //This should be the game manager disabling the player
            GameManager.Player.enabled = false;
            GameManager.Player.GetComponent<Rigidbody>().velocity = Vector3.zero;

            GameManager.displayDialog(name, dialog[dialogCounter]);
            dialogCounter++;
        }
	}
}
