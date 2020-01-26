using UnityEngine;
using System.Collections.Generic;

public class PatrolPath : MonoBehaviour {

    private List<PatrolNode> _path;
    public List<PatrolNode> Path
    {
        get
        {
            if (_path == null || _path.Count == 0)
            {
                RebuildPath();
            }
            return _path;
        }
    }
    public bool debugDraw = true;

    public PatrolNode nodeType;
    private Color DRAW_COLOR = Color.yellow;
    private int lastPathCount = 0;


    public void OnDrawGizmos()
    {
        if (_path == null || lastPathCount != transform.childCount)
        {
            RebuildPath();
        }

        if (debugDraw)
        {
            Gizmos.color = DRAW_COLOR;
            if(_path == null)
            {
                return;
            }
            for (int i = 0; i < this._path.Count; ++i)
            {
                // Check if the object still exists.
                // If it doesn't, rebuild the path.
                if (this._path[i] == null)
                {
                    this._path = null;
                    return;
                }
                if (i > 0)
                {
                    Gizmos.DrawLine(this._path[i - 1].transform.position, this._path[i].transform.position);
                }
                else if (i == 0)
                {
                    Gizmos.DrawLine(this._path[this._path.Count - 1].transform.position, this._path[0].transform.position);
                }
            }
        }
        
    }
    public void RebuildPath()
    {
        Debug.Log("Refreshing the patrol path for " + this.gameObject);

        this._path = new List<PatrolNode>();
        foreach (PatrolNode node in this.transform.GetComponentsInChildren<PatrolNode>())
        {
            this._path.Add(node);
        }
        lastPathCount = this._path.Count;
    }

	// Use this for initialization
	void Start () {
        RebuildPath();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
