using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndMatchPanel : MonoBehaviour {

    public delegate void ButtonClickEvent();
    public event ButtonClickEvent OnReturnButtonClick;

    public string text;
    private Text textComponent;

    void Start()
    {
        if (textComponent == null)
        {
            textComponent = this.transform.FindChild("ResultText").GetComponent<Text>();
        }
    }


	public void ReturnButtonClick()
    {
        if(OnReturnButtonClick != null)
        {
            OnReturnButtonClick();
        }
        Destroy(this.gameObject);
    }

    public void SetText(string p)
    {
        text = p;
        textComponent = this.transform.FindChild("ResultText").GetComponent<Text>();
        textComponent.text = text;
    }
}
