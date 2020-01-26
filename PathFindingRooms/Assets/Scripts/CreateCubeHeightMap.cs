using UnityEngine;
using System.Collections;

public class CreateCubeHeightMap : MonoBehaviour {

    public GameObject cubePrefab;

    public AnimationCurve parametricX, parametricY, oscillateX, oscillateY;
    public int resolution;

    public float cubeSize;
    public float heightScale;

    public float[,] heightMap { get; private set; }

    // Use this for initialization
    void Start() {
        heightMap = new float[resolution, resolution];
        for (int u = 0; u < resolution; u++) {
            for (int v = 0; v < resolution; v++) {
                float height = heightScale*combine(u, v);
                Vector3 position = new Vector3(u, height, v);
                GameObject cube = (GameObject)Instantiate(cubePrefab, cubeSize * position, Quaternion.identity);
                cube.transform.parent = transform;
                cube.name = "Cube " + (u * resolution + v);

                cube.GetComponent<OscillateUpDown>().frequency = combine2(u, v);

                heightMap[u, v] = height;
            }
        }

        cubePrefab.SetActive(false);
    }

    private float combine(float x, float y) {
        float fX = parametricX.Evaluate(x / resolution);
        float fY = parametricY.Evaluate(y / resolution);
        return Mathf.Max(fX, fY) - Mathf.Min(fX, fY);
    }

    private float combine2(float x, float y) {
        float fX = oscillateX.Evaluate(x / resolution);
        float fY = oscillateY.Evaluate(y / resolution);
        return Mathf.Min(fX, fY);
    }
}
