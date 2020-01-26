using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CollisionPrediction  {

    public static Vector3 AvoidCollisions(GameObject self,
        float detectRadius,
        float avoidMargin,
        float speed,
        int mask = Physics.DefaultRaycastLayers,
        Collider[] selfColliders = null)
    {
        Vector3 results = new Vector3();
        Rigidbody selfBody = self.GetComponent<Rigidbody>();

        // Cannot operate if we have no sense of self movement
        if(selfBody == null)
        {
            return results;
        }

        if(selfColliders == null)
        {
            selfColliders = self.GetComponentsInChildren<Collider>();
        }

        Collider[] nearbyColliders = Physics.OverlapSphere(selfBody.position, detectRadius, mask);
        foreach (Collider col in nearbyColliders)
        {
            // Ignore colliders that belong to self
            bool skipSelf = false;
            foreach(Collider c in selfColliders)
            {
                if(col == c)
                {
                    skipSelf = true;
                    break;
                }
            }
            if(skipSelf)
            {
                continue;
            }

            // Ignore terrain (probably unneeded)
            if(col.gameObject.tag.Equals("Terrain"))
            {
                continue;
            }

            // Moving obstacles
            if (col.gameObject.GetComponent<Rigidbody>() != null)
            {
                // self = A
                // other = B
                Rigidbody other = col.gameObject.GetComponent<Rigidbody>();
                Vector3 dv = other.velocity - selfBody.velocity;
                Vector3 dp = other.position - selfBody.position;
                float closestTime = -(Vector3.Dot(dv, dp) / dv.sqrMagnitude);

                Vector3 closestA = selfBody.position + selfBody.velocity * closestTime;
                Vector3 closestB = other.position + other.velocity * closestTime;

                Vector3 distance = closestB - closestA;
                if (distance.sqrMagnitude < avoidMargin * avoidMargin)
                {
                    Debug.Log("Avoiding " + other.gameObject.name);
                    results += 0.5f * (avoidMargin / distance.magnitude) * StaticMovementAlgorithms.KinematicFlee(selfBody, closestB, speed);
                }
                continue;
            }
        }

        // Not really worried about static obstacles if we're not moving
        if (selfBody.velocity.sqrMagnitude > 0.0001)
        {
            RaycastHit[] hitInfos = selfBody.SweepTestAll(selfBody.velocity.normalized, detectRadius);
            if (hitInfos.Length > 0)
            {
                foreach (RaycastHit hitInfo in hitInfos)
                {
                    if ((mask & (1 << hitInfo.collider.gameObject.layer)) > 0)
                    {
                        Vector3 distance = hitInfo.point - selfBody.position;
                        if (distance.sqrMagnitude < avoidMargin * avoidMargin)
                        {
                            Debug.Log("Avoiding " + hitInfo.collider.gameObject.name);
                            Vector3 dest = hitInfo.point + hitInfo.normal * 2.0f * avoidMargin;
                            results += (avoidMargin / distance.magnitude) *  StaticMovementAlgorithms.KinematicSeek(selfBody, dest, 1.0f);
                        }
                        break;
                    }
                }
            }
        }

        return results.normalized * speed;
    }

}
