using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Formations {

    private static IEnumerable<Vector3> CircleFormation(Vector3 leaderPosition, Vector3 centerPosition, Vector3 circleNormal, int amount) {
        yield return leaderPosition;
        if (amount > 1) {
            Vector3 X = leaderPosition - centerPosition;
            Vector3 Y = Vector3.Cross(X, Vector3.ProjectOnPlane(circleNormal, leaderPosition - centerPosition).normalized);
            for (int i = 1; i < amount; i++) {
                float t = 2 * Mathf.PI * i / amount;
                yield return Mathf.Sin(t) * Y + Mathf.Cos(t) * X + centerPosition;
            }
        }
    }

    public static List<Vector3> GetCircleFormation(Vector3 leaderPosition, Vector3 centerPosition, Vector3 circleNormal, int amount) {
        return CircleFormation(leaderPosition, centerPosition, circleNormal, amount).ToList<Vector3>();
    }


    // Reference: http://math.stackexchange.com/questions/41940/is-there-an-equation-to-describe-regular-polygons/41954#41954
    private static IEnumerable<Vector3> PolygonFormation(Vector3 leaderPosition, Vector3 centerPosition, Vector3 polyNormal, int numberOfSides, int amount) {
        yield return leaderPosition;
        if (amount > 1) {
            Vector3 X = leaderPosition - centerPosition;
            Vector3 Y = Vector3.Cross(X, Vector3.ProjectOnPlane(polyNormal, leaderPosition - centerPosition).normalized);

            //float totalAmount = (amount - 1);// *numberOfSides;

            for (int i = 1; i < amount; i++) {
                float t = 2 * Mathf.PI * i / amount;
                float radius = Mathf.Cos(Mathf.PI / numberOfSides) / Mathf.Cos(t % (2 * Mathf.PI / numberOfSides) - Mathf.PI / numberOfSides);
                yield return radius * (Mathf.Cos(t) * X + Mathf.Sin(t) * Y) + centerPosition;
            }
        }
    }

    public static List<Vector3> GetPolygonFormation(Vector3 leaderPosition, Vector3 centerPosition, Vector3 polyNormal, int numberOfSides, int amount) {
        return PolygonFormation(leaderPosition, centerPosition, polyNormal, numberOfSides, amount).ToList<Vector3>();
    }

    
    public static List<Vector3> GetVFormation(Vector3 leaderPosition, float spread, int amount)
    {
        List<Vector3> Positions = new List<Vector3>();

        for (int i = 0; i < amount; i++)
        {
            Vector3 position;
            if ((i % 2) == 0)
            {
                position = new Vector3(1 * (i + 1) / 2 * spread, 0, -(i + 1) / 2 * spread);
            }
            else
            {
                position = new Vector3(-1 * (i + 1) / 2 * spread, 0, -(i + 1) / 2 * spread);
            }
            Positions.Add(position);
        }
        return Positions;
    }
}
