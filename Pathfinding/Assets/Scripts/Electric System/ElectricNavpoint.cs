using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ElectricNavpoint : MonoBehaviour {
    public List<ElectricNavpoint> adjacentPoints;
    public bool isEndpoint;

    //TODO: Implement something where going over the navpoints disables devices
    public List<ElectricDevice> associatedDevices;

    void Update()
    {
        foreach (ElectricNavpoint navpoint in adjacentPoints)
        {
            if(navpoint.adjacentPoints.Contains(this))
                Debug.DrawLine(this.transform.position, navpoint.transform.position);
        }
    }

    public ElectricNavpoint getNextNavPoint(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
             ElectricNavpoint nextNavPoint = adjacentPoints.Aggregate(
                (nav1, nav2) => Vector3.Angle(nav1.transform.position - gameObject.transform.position, direction) < Vector3.Angle(nav2.transform.position - gameObject.transform.position, direction)
                    ? nav1 : nav2);
             return nextNavPoint;
        }
        else
        {
            return adjacentPoints[0];
        }
    }

    //This will disable the associated electric device
    public void trigger() {
        foreach(ElectricDevice device in associatedDevices)
        {
            device.disable();
        }
    }

    internal ElectricNavpoint getFirstNavPoint()
    {
        return adjacentPoints[0];
    }
}
