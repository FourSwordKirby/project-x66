using UnityEngine;
using System.Collections;

public enum SecurityMemberStatus
{
    Unengaged,
    Engaged
}

public abstract class SecurityMember : Mobile {
    public abstract void Alert(GameObject target);
    public abstract void ReturnToPosition();
    public abstract SecurityMemberStatus GetStatus(GameObject target);
}
