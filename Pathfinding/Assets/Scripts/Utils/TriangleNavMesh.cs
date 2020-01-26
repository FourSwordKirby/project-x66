using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TriangleNavMesh : MonoBehaviour, INavMesh {

    public class Triangle : INavCell {
        private Vector3 a, b, c;

        public Vector3 center
        {
            get;
            private set;
        }

        public List<INavCell> neighbors
        {
            get;
            private set;
        }

        public Dictionary<string, object> properties
        {
            get;
            private set;
        }

        public Triangle(Vector3 a, Vector3 b, Vector3 c) {
            this.a = a;
            this.b = b;
            this.c = c;

            this.center = (a + b + c) / 3;

            neighbors = new List<INavCell>();
            properties = new Dictionary<string, object>();
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

        public bool isAdjacent(INavCell other)
        {
            if (!(other is Triangle))
            {
                return false;
            }
            return this.isAdjacent((Triangle)other);
        }

        private bool isAdjacent(Triangle other) {
            return sharedVertices(other) == 2;
        }

        public override bool Equals(object other) {
            if (!(other is Triangle)) {
                return false;
            }
            return sharedVertices((Triangle)other) == 3;
        }

        public void addNeighbor(INavCell other) {
            if (!neighbors.Contains(other)) {
                neighbors.Add(other);
            }
        }
    }

    public Mesh mesh;

    private List<Triangle> triangles;

    void Awake() {
        triangles = new List<Triangle>();

        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            Vector3 a = transform.TransformPoint(mesh.vertices[mesh.triangles[i]]);
            Vector3 b = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 1]]);
            Vector3 c = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 2]]);
            Triangle t = new Triangle(a, b, c);
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


    private Triangle getClosestTriangle(Vector3 v) {
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

    public INavCell getClosestNavCell(Vector3 v)
    {
        return (INavCell)getClosestTriangle(v);
    }

    public List<INavCell> getAllNavCells()
    {
        return triangles.Select(x => (INavCell)x).ToList();
    }
}
