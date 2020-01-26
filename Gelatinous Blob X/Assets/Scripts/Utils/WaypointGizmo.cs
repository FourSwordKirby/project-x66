using UnityEngine;
using System.Collections;

public class WaypointGizmo : MonoBehaviour {

    public Mesh mesh;
    
    void OnDrawGizmos()
    {
        Gizmos.DrawMesh(mesh, transform.position);
    }

}
