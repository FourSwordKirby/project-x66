using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CollisionPrediction  {

    public static Vector3 AvoidCollisionsHelper(GameObject self,
        float detectRadius,
        float avoidMargin,
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
        Vector3 currentDirection = selfBody.velocity.normalized;

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

            // Moving obstacles
            if (col.gameObject.GetComponent<Rigidbody>() != null)
            {
                // self = A
                // other = B
                Rigidbody other = col.gameObject.GetComponent<Rigidbody>();
                if(other.velocity.sqrMagnitude < 0.01f)
                {
                    continue;
                }

                Vector3 dv = other.velocity - selfBody.velocity;
                Vector3 dp = other.position - selfBody.position;
                float closestTime = -(Vector3.Dot(dv, dp) / dv.sqrMagnitude);

                Vector3 closestA = selfBody.position + selfBody.velocity * closestTime;
                Vector3 closestB = other.position + other.velocity * closestTime;

                Vector3 distance = closestB - closestA;
                if (distance.sqrMagnitude < avoidMargin * avoidMargin)
                {
                    //Debug.Log("Avoiding " + other.gameObject.name);
                    Vector3 deltaV = StaticMovementAlgorithms.KinematicFlee(selfBody, closestB, 1.0f) - currentDirection;
                    deltaV.Normalize();
                    float proportionalToDist = (1.0f - distance.magnitude / avoidMargin);
                    deltaV *= proportionalToDist;

                    // Scale by distance
                    results += deltaV;
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
                        Vector3 displacement = hitInfo.point - selfBody.position;
                        if (displacement.sqrMagnitude < detectRadius * detectRadius) // Within Avoid Margin
                        {
                            //Debug.Log("Avoiding " + hitInfo.collider.gameObject.name);
                            //Vector3 avoidPoint = hitInfo.point + hitInfo.normal * avoidMargin;
                            //Vector3 targetV = StaticMovementAlgorithms.KinematicSeek(selfBody, avoidPoint, 1.0f);
                            //Vector3 deltaV = (targetV - currentDirection);
                            //if(deltaV.magnitude > 1.0f)
                            //{
                            //    deltaV.Normalize();
                            //}
                            Vector3 up = Vector3.Cross(currentDirection, hitInfo.normal);
                            Vector3 avoidDirection = Vector3.Cross(up, currentDirection).normalized;
                            avoidDirection.y = 0.0f;
                            Vector3 deltaV = avoidDirection.normalized;

                            float proportionalToDist = (1.0f - displacement.magnitude / (detectRadius));
                            deltaV *= proportionalToDist;

                            results += deltaV;
                        }
                        break;
                    }
                }
            }
        }

        return results;
    }

    public static Vector3 AvoidCollisions(GameObject self,
        float detectRadius,
        float avoidMargin,
        float speed,
        int mask = Physics.DefaultRaycastLayers,
        Collider[] selfColliders = null)
    {
        return AvoidCollisionsHelper(self, detectRadius, avoidMargin, mask, selfColliders).normalized * speed;
    }
}
