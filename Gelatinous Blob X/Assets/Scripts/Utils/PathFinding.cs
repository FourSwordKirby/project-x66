using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PathFinding : MonoBehaviour {

    public static INavMesh levelNavMesh;
    public static Dictionary<GameObject, Dictionary<string, Dictionary<INavCell, float>>> visitedDistancesCache;
    public static Dictionary<GameObject, Vector3> lastLocationCache;

    private const float REPATH_THRESHOLD = 2.0f;

    public delegate float HeuristicFunction(Vector3 source, Vector3 target);
    public delegate float CostFunction(INavCell source, INavCell target);

    void Awake()
    {
        visitedDistancesCache = new Dictionary<GameObject, Dictionary<string, Dictionary<INavCell, float>>>();
        lastLocationCache = new Dictionary<GameObject, Vector3>();
    }

    void Start()
    {
        levelNavMesh = GameObject.FindObjectOfType<TriangleNavMesh>();
        Debug.Log("levelNavMesh came back as " + levelNavMesh);
    }

    private static float euclideanHeuristic(Vector3 source, Vector3 target)
    {
        return Vector3.Distance(source, target);
    }

    private static float euclideanCost(INavCell source, INavCell target)
    {
        return Vector3.Distance(source.center, target.center);
    }

    public static List<GameObject> spawnGameObjectsAtPathPoints(List<Vector3> path)
    {
        // Holds the path points that we're about to spawn so that the editor hierarchy doesn't go crazy.
        GameObject pathPoints = new GameObject("Path points");
        var conversion = path.Select(point =>
        {
            GameObject obj = new GameObject();
            obj.transform.position = point;
            obj.transform.parent = pathPoints.transform;
            return obj;
        });
        return new List<GameObject>(conversion);
    }

    public static List<Vector3> generatePath(GameObject source,
        GameObject target,
        CostFunction computeCost = null,
        HeuristicFunction computeHeuristic = null)
    {
        if(computeCost == null)
        {
            computeCost = euclideanCost;
        }
        if(computeHeuristic == null)
        {
            computeHeuristic = euclideanHeuristic;
        }
        string costFunctionID = computeCost.Method.ToString();

        if(levelNavMesh == null)
        {
            Debug.Log("Could not find a CustomNavMesh");
            List<Vector3> l = new List<Vector3>();
            l.Add(target.transform.position);
            return l;
        }

        //Debug.Log("Checking cache...");
        if(lastLocationCache.ContainsKey(target))
        {
            if (Vector3.Distance(target.transform.position, lastLocationCache[target]) > REPATH_THRESHOLD)
            {
                visitedDistancesCache.Remove(target);
            }
        }

        Dictionary<INavCell, float> visitedDistances;
        if(visitedDistancesCache.ContainsKey(target))
        {
            var funcToDistances = visitedDistancesCache[target];
            if(funcToDistances.ContainsKey(costFunctionID))
            {
                visitedDistances = funcToDistances[costFunctionID];
            }
            else
            {
                //Debug.Log("Cannot find cost function");
                visitedDistances = new Dictionary<INavCell, float>();
            }
        }
        else
        {
            visitedDistances = new Dictionary<INavCell, float>();
        }

        //Debug.Log("Generate path starting...");

        // Backwards A*
        INavCell targetCell = levelNavMesh.getClosestNavCell(target.transform.position);
        Vector3 startPosition = source.transform.position;
        List<INavCell> startCells = levelNavMesh.getClosestNavCell(startPosition).neighbors;

        if(!startCells.Exists(cell => visitedDistances.ContainsKey(cell)))
        {
            //DateTime startTime = DateTime.UtcNow;

            PriorityQueue<INavCell> pQueue = new PriorityQueue<INavCell>();

            if (visitedDistances.Count == 0)
            {
                pQueue.Add(0.0f + computeHeuristic(targetCell.center, startPosition), targetCell);
            }
            else
            {
                foreach (var kvp in visitedDistances)
                {
                    INavCell cell = kvp.Key;
                    float gValue = kvp.Value;
                    foreach (INavCell neighbor in cell.neighbors)
                    {
                        if (visitedDistances.ContainsKey(neighbor))
                        {
                            continue;
                        }

                        // Edge weight
                        float weight = computeCost(neighbor, cell);

                        // A* Heuristic -- Euclidean distance
                        float heuristic = computeHeuristic(neighbor.center, startPosition);
                        pQueue.Add(gValue + weight + heuristic, neighbor);
                    }
                }
            }

            PriorityQueue<INavCell>.Node currentNode = pQueue.RemoveMin();

            // Debug
            //GameObject aStarVisited = new GameObject("A-Star Visits");

            //float counter = 0;

            while (!startCells.Contains(currentNode.Value)) //&& counter < 10.0f)
            {
                INavCell currentNavCell = currentNode.Value;

                //Update our visited things
                if (!visitedDistances.ContainsKey(currentNavCell))
                {
                    // Hmm, by adding the heuristic to the priority,
                    // we now have that priority != distance traveled
                    float gValue = currentNode.Priority - computeHeuristic(currentNavCell.center, startPosition);
                    visitedDistances.Add(currentNavCell, gValue);

                    // Debug
                    //GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //Destroy(marker.GetComponent<Collider>());
                    //marker.transform.position = currentNavCell.center;
                    //marker.transform.parent = aStarVisited.transform;

                    List<INavCell> neighbors = currentNavCell.neighbors;

                    foreach (INavCell neighbor in neighbors)
                    {
                        // Edge weight
                        float weight = computeCost(neighbor, currentNavCell);

                        // A* Heuristic -- Euclidean distance
                        float heuristic = computeHeuristic(neighbor.center, startPosition);
                        pQueue.Add(gValue + weight + heuristic, neighbor);
                    }
                }

                //Now move onto the next thing in the PQ
                currentNode = pQueue.RemoveMin();
            }

            //final addition of the last node
            visitedDistances.Add(currentNode.Value, currentNode.Priority - computeHeuristic(currentNode.Value.center, startPosition));

            //Debug.Log("A* search done in " + (DateTime.UtcNow - startTime).Milliseconds);

            // Caching
            if (!visitedDistancesCache.ContainsKey(target))
            {
                var funcToDist = new Dictionary<string, Dictionary<INavCell, float>>();
                funcToDist.Add(costFunctionID, visitedDistances);
                visitedDistancesCache.Add(target, funcToDist);
            }
            else
            {
                var funcToDist = visitedDistancesCache[target];
                if (!funcToDist.ContainsKey(costFunctionID))
                {
                    funcToDist.Add(costFunctionID, visitedDistances);
                }
                else
                {
                    funcToDist[costFunctionID] = visitedDistances;
                }
            }

            if (!lastLocationCache.ContainsKey(target))
            {
                lastLocationCache.Add(target, target.transform.position);
            }
            else
            {
                lastLocationCache[target] = target.transform.position;
            }
        }

        //if (counter > 10.0f)
        //    Debug.Log("timed out");

        //Debug.Log("A* done");

        List<INavCell> cellPath = new List<INavCell>();
        INavCell backTracer = startCells.First(cell => visitedDistances.ContainsKey(cell));
        float backTracerDistace = visitedDistances[backTracer];
        cellPath.Add(backTracer);

        // Debug
        //GameObject chosenPath = new GameObject("Chosen path");

        while (backTracer != targetCell)
        {
            int count = cellPath.Count;
            List<INavCell> neighbors = backTracer.neighbors;
            foreach (INavCell neighbor in neighbors)
            {
                if (visitedDistances.ContainsKey(neighbor))
                {
                    float weight = computeCost(neighbor, backTracer);
                    if (Mathf.Abs(backTracerDistace - weight - visitedDistances[neighbor]) < 0.05f)
                    {
                        //update stuff about the backtracer
                        backTracer = neighbor;
                        backTracerDistace = visitedDistances[neighbor];

                        //GameObject node = new GameObject();
                        //node.transform.position = neighbor.center;
                        //polygonPath.Add(node);
                        cellPath.Add(neighbor);

                        // Debug
                        //GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        //Destroy(marker.GetComponent<Collider>());
                        //marker.transform.position = neighbor.center;
                        //marker.transform.parent = chosenPath.transform;
                        continue;
                    }
                }
            }
            if (count == cellPath.Count)
            {
                //if we made it here, something failed
                //Debug.Log("dying");
                break;
            }
        }
        //Debug.Log("Backtracing done");

        //GameObject startNode = new GameObject();
        //startNode.transform.position = start.center;
        cellPath.Add(targetCell);
        //cellPath.Reverse();
        //Debug.Log("Backtracing got path length " + cellPath.Count);

        List<Vector3> resultPath;
        Bounds b = new Bounds();
        foreach(Collider c in source.GetComponentsInChildren<Collider>())
        {
            if(!c.isTrigger)
            {
                b.Encapsulate(c.bounds);
            }
        }
        //Debug.Log("Determined radius: " + b.extents.magnitude);
        resultPath = SmoothPathByShortCutting(cellPath, LayerMask.GetMask("Obstacle", "Ground"), b.extents.magnitude);
        //Debug.Log("Smoothing done");


        //resultPath = new List<GameObject>(polygonPath.Select(s =>
        //{
        //    GameObject obj = new GameObject();
        //    obj.transform.position = s.center;
        //    return obj;
        //}));

        return resultPath;
    }

    /// <summary>
    /// Smooths a path by shortcutting the paths when no obstacles are in the way.
    /// Detects for obstacles on the layers specified by the collisionMask.
    /// Can take in a body radius to perform a SphereCast for a better assessment of collisions.
    /// NOTE: Current spawns GameObjects everywhere. Might want to move that GameObject spawning elsewhere.
    /// </summary>
    public static List<Vector3> SmoothPathByShortCutting(List<INavCell> path,
        int collisionMask = Physics.DefaultRaycastLayers,
        float bodyRadius = 0.0f)
    {
        List<Vector3> result = new List<Vector3>();
        int startIndex = 0;
        for (int i = 0; i < path.Count; ++i)
        {
            // So long as we're not at the goal, determine if we can skip this path point
            if (i < path.Count - 1)
            {
                Vector3 pathStart = path[startIndex].center;
                Vector3 pathEnd = path[i + 1].center;         // See if we can get to the next path point without problem

                // The following nested-if block will determine if we can cut path point i
                // Note: All y movements will need pathpoints, so we can only cut out
                // points that are along the same y plane
                if(Mathf.Abs(pathEnd.y - pathStart.y) < 0.01f)
                {
                    // Use spherecasting
                    if (bodyRadius > 0.0f)
                    {
                        Vector3 displacement = pathEnd - pathStart;
                        Vector3 direction = displacement.normalized;
                        Ray r = new Ray(pathStart, direction);
                        float distance = displacement.magnitude;
                        if (!Physics.SphereCast(r, bodyRadius, distance, collisionMask))
                        {
                            // No obstruction, can path point at i
                            continue;
                        }
                    }

                    // Use rigidbody cast
                    else
                    {
                        if (!Physics.Linecast(pathStart, pathEnd, collisionMask))
                        {
                            // Obstacle free! Don't add the path point at i
                            continue;
                        }
                    }
                }
            }

            Vector3 relevantPoint = path[i].center;
            startIndex = i;
            result.Add(relevantPoint);
        }
        return result;
    }
}
