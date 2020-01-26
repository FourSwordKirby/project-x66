using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireNavMeshArea : NavMeshArea {

	// Use this for initialization
	void Awake () {
        INavMesh levelMesh = GameObject.FindObjectOfType<TriangleNavMesh>();

        List<INavCell> allCells = levelMesh.getAllNavCells();
        foreach(INavCell cell in allCells)
        {
            setProperties(cell);
        }
	}

    override public void setProperties(INavCell cell)
    {
        if (gameObject.GetComponent<BoxCollider>().bounds.Contains(cell.center))
        {
            cell.properties.Add("OnFire", true);
            Debug.Log(cell.properties.ContainsKey("OnFire"));
        }
    }
}
