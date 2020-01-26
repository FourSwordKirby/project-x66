using UnityEngine;
using System.Collections;

public abstract class Mobile : MonoBehaviour{

    protected State<Mobile> state;

    protected bool invulnerable;            //Used to give the mobile temporary invincibility after getting hit etc.

    public abstract void ChangeState(State<Mobile> state);

}
