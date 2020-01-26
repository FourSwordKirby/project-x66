using UnityEngine;
using System.Collections;

public class TestFormations : MonoBehaviour {

    public Transform leader;
    public Transform center;
    public Transform normal;
    public int numberOfSides;
    public int amount;
	
	// Update is called once per frame
	void Update () {
        foreach (Vector3 point in Formations.GetPolygonFormation(leader.position, center.position, normal.position - center.position, numberOfSides, amount)) {
            Debug.DrawLine(center.position, point, Color.red);
        }
        foreach (Vector3 point in Formations.GetCircleFormation(leader.position, center.position, normal.position - center.position, amount)) {
            Debug.DrawLine(center.position, point, Color.blue);
        }
	}
}
