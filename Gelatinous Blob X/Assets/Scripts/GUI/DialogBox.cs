using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class DialogBox : MonoBehaviour {
    public Text nameLabel;
    public Text dialogField;

    private string name = "";
    private string dialog = "";
    private int dialogTracker = 0;

    private float textDisplaySpeed;
    private float textDisplayTimer;

    private const  float FAST_DISPLAY_SPEED = 0.0f;
    private const float SLOW_DISPLAY_SPEED = 0.03f;

    // Use this for initialization
	void Start () {
        this.nameLabel.text = "";
  	    this.dialogField.text = "";
	}
	// Update is called once per frame
	void Update () {
        //Do something where text appears according to the textDisplaySpeed
        if (textDisplayTimer> 0)
        {
            textDisplayTimer -= Time.deltaTime;
            return;
        }

        this.nameLabel.text = name;
        if (this.dialogField.text != dialog)
        {
            Debug.Log(this.dialogField.text);
            Debug.Log(dialog);
            this.dialogField.text += dialog[dialogTracker];
            dialogTracker++;

            textDisplayTimer = textDisplaySpeed;
        };
	}

    public void displayDialog(string name, string dialog, DisplaySpeed displaySpeed = DisplaySpeed.fast)
    {
        this.gameObject.SetActive(true);
        this.name = name;
        this.dialog = dialog;
        this.dialogTracker = 0;

        //Prevents the name from flickering
        if (this.nameLabel.text != name)
            this.nameLabel.text = "";
        this.dialogField.text = "";

        if (displaySpeed == DisplaySpeed.immediate)
        {
            this.dialogField.text = dialog;
        }
        else if (displaySpeed == DisplaySpeed.fast)
        {
            textDisplaySpeed = FAST_DISPLAY_SPEED;
        }
        else if (displaySpeed == DisplaySpeed.slow)
        {
            textDisplaySpeed = SLOW_DISPLAY_SPEED;
        }
    }

    public void closeDialog()
    {
        this.gameObject.SetActive(false);
        this.name = "";
        this.dialog = "";
        this.dialogTracker = 0;
    }
}

public enum DisplaySpeed{
    immediate,
    slow,
    fast
}