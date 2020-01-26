using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour {

    private SecurityDoor associatedDoor;

    float coolDown;

    void Start()
    {
        associatedDoor = this.GetComponentInParent<SecurityDoor>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<Player>() != null || col.gameObject.GetComponent<Enemy>() != null)
        {
            associatedDoor.Open();
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<Player>() != null || col.gameObject.GetComponent<Enemy>() != null)
        {
            associatedDoor.Open();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<Player>() != null || col.gameObject.GetComponent<Enemy>() != null)
        {
            associatedDoor.Close();
        }
    }
}
