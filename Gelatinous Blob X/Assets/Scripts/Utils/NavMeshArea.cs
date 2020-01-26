using UnityEngine;
using System.Collections;

public abstract class NavMeshArea : MonoBehaviour {

    public abstract void setProperties(INavCell cell);
}
