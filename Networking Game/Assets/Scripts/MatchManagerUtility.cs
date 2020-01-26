using UnityEngine;
using System.Collections.Generic;

public class MatchRequest
{
    public static float REQUEST_TIMEOUT = 15.0f;

    public RpsAgent initiator;
    public RpsAgent victim;
    public float timeRemaining;

    public MatchRequest(RpsAgent p1, RpsAgent p2)
    {
        initiator = p1;
        victim = p2;
        timeRemaining = REQUEST_TIMEOUT;
    }
}