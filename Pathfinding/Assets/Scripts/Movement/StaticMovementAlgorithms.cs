using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class StaticMovementAlgorithms {
    public static bool lockX { private get; set; }
    public static bool lockY { private get; set; }
    public static bool lockZ { private get; set; }

    public static Vector3 KinematicSeek(Rigidbody rigidbody, Vector3 target, float speed) {
        Vector3 velocity = (target - rigidbody.position).normalized * speed;
        velocity.Scale(LockDimensions);
        return velocity;
    }
    public static Vector3 KinematicFlee(Rigidbody rigidbody, Vector3 target, float speed) {
        Vector3 velocity = (rigidbody.position - target).normalized * speed;
        //Debug.Log(velocity);
        //velocity.Scale(LockDimensions);
        return velocity;
    }
    public static Vector3 KinematicArrive(Rigidbody rigidbody, Vector3 target, float speed, float arrivalRadius) {
        Vector3 offset = target - rigidbody.position;

        if (offset.sqrMagnitude < arrivalRadius * arrivalRadius)
        {
            return Vector3.zero;
        }
        else
        {
            return offset.normalized * speed;
        }
    }
    public static Vector3 KinematicWander(Rigidbody rigidbody, float speed, float movementVariance, Vector3 orientation) {
        /*Not correct
        Vector3 currentDirection = rigidbody.velocity.normalized;
        float dX = Mathf.Sqrt(Random.Range(0f, 1f)) - 0.5f;
        float dY = Mathf.Sqrt(Random.Range(0f, 1f)) - 0.5f;
        float dZ = Mathf.Sqrt(Random.Range(0f, 1f)) - 0.5f;

        Vector3 newDirection = currentDirection + new Vector3(dX, dY, dZ);

        Vector3 velocity = newDirection.normalized * speed;

        return velocity;
         */
        float orientationVariance = Random.Range(-movementVariance, movementVariance);
        Vector3 newOrientation = Quaternion.AngleAxis(orientationVariance, Vector3.up) * orientation;

        return speed * newOrientation.normalized;
    }
    public static Vector3 KinematicFleeMultiple(Rigidbody rigidbody, List<Vector3> targets, float speed, float maxWeight) {
        Vector3 totalVelocity = targets.Select(x => Mathf.Min(maxWeight, 1 / (rigidbody.position - x).sqrMagnitude) * KinematicFlee(rigidbody, x, speed)).Aggregate((x, y) => x + y);
        float totalWeight = targets.Select(x => Mathf.Min(maxWeight, 1 / (rigidbody.position - x).sqrMagnitude)).Aggregate((x, y) => x + y);

        return totalVelocity / totalWeight;
    }
    public static Vector3 KinematicAvoidObstacles(Rigidbody rigidbody, LayerMask obstacles, float raycastLength, float playerRadius, float speed) {

        Vector3 forward = rigidbody.transform.forward;
        Vector3 right = rigidbody.transform.right;
        Vector3 up = rigidbody.transform.up;
        RaycastHit raycastInfo;
        Ray ray = new Ray(rigidbody.position, forward);
        if (Physics.Raycast(rigidbody.position, forward, out raycastInfo, raycastLength, obstacles)) {
            return KinematicSeek(rigidbody, raycastInfo.point + raycastInfo.normal * playerRadius, speed);
        }
        else if (Physics.Raycast(rigidbody.position, forward + playerRadius/10 * right, out raycastInfo, raycastLength, obstacles)) {
            return KinematicSeek(rigidbody, raycastInfo.point + raycastInfo.normal * playerRadius, speed);
        }
        else if (Physics.Raycast(rigidbody.position, forward - playerRadius / 10 * right, out raycastInfo, raycastLength, obstacles)) {
            return KinematicSeek(rigidbody, raycastInfo.point + raycastInfo.normal * playerRadius, speed);
        }
        else if (Physics.Raycast(rigidbody.position, forward + playerRadius / 10 * up, out raycastInfo, raycastLength, obstacles)) {
            return KinematicSeek(rigidbody, raycastInfo.point + raycastInfo.normal * playerRadius, speed);
        }
        else if (Physics.Raycast(rigidbody.position, forward - playerRadius / 10 * up, out raycastInfo, raycastLength, obstacles)) {
            return KinematicSeek(rigidbody, raycastInfo.point + raycastInfo.normal * playerRadius, speed);
        }
        else if (Physics.Raycast(rigidbody.position + playerRadius * right, forward, out raycastInfo, raycastLength, obstacles)) {
            return KinematicSeek(rigidbody, raycastInfo.point + raycastInfo.normal * playerRadius, speed);
        }
        else if (Physics.Raycast(rigidbody.position - playerRadius * right, forward, out raycastInfo, raycastLength, obstacles)) {
            return KinematicSeek(rigidbody, raycastInfo.point + raycastInfo.normal * playerRadius, speed);
        }
        else if (Physics.Raycast(rigidbody.position + playerRadius * up, forward, out raycastInfo, raycastLength, obstacles)) {
            return KinematicSeek(rigidbody, raycastInfo.point + raycastInfo.normal * playerRadius, speed);
        }
        else if (Physics.Raycast(rigidbody.position - playerRadius * up, forward, out raycastInfo, raycastLength, obstacles)) {
            return KinematicSeek(rigidbody, raycastInfo.point + raycastInfo.normal * playerRadius, speed);
        }
        else {
            //possibly return a 0 vector instead due to be used by other behaviors
            //return KinematicSeek(rigidbody, rigidbody.position + rigidbody.transform.forward * raycastLength, speed);
            //return rigidbody.velocity;
            return Vector3.zero;
        }
    }

    public static Vector3 GetInterpolatedVelocity(Rigidbody rigidbody, Vector3 newVelocity, float interpolationAmount) {
        return Vector3.Lerp(rigidbody.velocity, newVelocity, interpolationAmount * Time.deltaTime);
        //return Vector3.MoveTowards(rigidbody.velocity, newVelocity, maxAcceleration);
    }
    public static Quaternion GetOrientation(Vector3 velocity) {
        return Quaternion.FromToRotation(Vector3.right, velocity);
    }
    public static Quaternion GetOrientation(Vector3 velocity, Vector3 baseOrientation) {
        return Quaternion.FromToRotation(baseOrientation, velocity);
        //return Quaternion.Lerp(baseOrientation, Quaternion.LookRotation(velocity), maxDegreesDelta);
    }

    private static Vector3 LockDimensions {
        get {
            return new Vector3(lockX ? 0 : 1, lockY ? 0 : 1, lockZ ? 0 : 1);
        }
    }
}
