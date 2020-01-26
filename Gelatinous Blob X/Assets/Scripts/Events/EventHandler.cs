using UnityEngine;
using System.Collections;

public class EventHandler : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Do somethings for different events

    //Initializes the player to have no stamina
    public static void removePlayerElectricPower(){
        GameManager.Player.SetStaminaRecovery(false);
        GameManager.Player.UseStamina(GameManager.Player.stamina);
    }
}
