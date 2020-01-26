using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {

    private Player p;
    private Image bar;

	void Start ()
    {
        p = GameManager.Player;
        if(p == null)
        {
            p = GameObject.FindObjectOfType<Player>();
        }
        bar = this.transform.FindChild("healthbar_front").GetComponent<Image>();
	}

    void Update()
    {
        bar.fillAmount = (float)p.health / p.maxHealth;
    }
}
