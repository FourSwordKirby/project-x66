using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridNavMesh : MonoBehaviour {

    public class Triangle {
        private Vector3 a, b, c;
        public Vector3 center;

        public List<Triangle> neighbors;

        public Triangle(Vector3 a, Vector3 b, Vector3 c) {
            this.a = a;
            this.b = b;
            this.c = c;

            this.center = (a + b + c) / 3;

            neighbors = new List<Triangle>();
        }

        private int sharedVertices(Triangle other) {
            int commonVertices = 0;

            if (other.a == a || other.b == a || other.c == a) {
                commonVertices++;
            }
            if (other.a == b || other.b == b || other.c == b) {
                commonVertices++;
            }
            if (other.a == c || other.b == c || other.c == c) {
                commonVertices++;
            }

            return commonVertices;
        }

        public bool isAdjacent(Triangle other) {
            return sharedVertices(other) == 2;
        }

        public override bool Equals(object other) {
            if (!(other is Triangle)) {
                return false;
            }
            return sharedVertices((Triangle)other) == 3;
        }

        public void addNeighbor(Triangle other) {
            if (!neighbors.Contains(other)) {
                neighbors.Add(other);
            }
        }
    }

    public Mesh mesh;

    private List<Triangle> triangles;

    void Start() {
        triangles = new List<Triangle>();

        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            Vector3 a = transform.TransformPoint(mesh.vertices[mesh.triangles[i]]);
            Vector3 b = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 1]]);
            Vector3 c = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 2]]);
            Triangle t = new Triangle(a, b, c); ;
            triangles.Add(t);
        }

        for (int i = 0; i < triangles.Count; i++) {
            for (int j = i + 1; j < triangles.Count; j++) {
                Triangle t1 = triangles[i];
                Triangle t2 = triangles[j];

                if (t1.isAdjacent(t2)) {
                    t1.addNeighbor(t2);
                    t2.addNeighbor(t1);
                    Debug.DrawLine(t1.center, t2.center, Color.blue, 100);
                }
            }
        }
    }

    public Triangle getClosestTriangle(Vector3 v) {
        if (triangles.Count < 1) {
            return null;
        }
        Triangle closestTriangle = triangles[0];
        float closestDistance = (closestTriangle.center - v).sqrMagnitude;

        for (int i = 1; i < triangles.Count; i++) {
            float distance = (triangles[i].center - v).sqrMagnitude;

            if (distance < closestDistance) {
                closestTriangle = triangles[i];
                closestDistance = distance;
            }
        }

        return closestTriangle;
    }
}
