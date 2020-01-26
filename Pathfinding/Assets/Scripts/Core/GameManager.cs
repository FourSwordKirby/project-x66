using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    private static GameManager instance;

    /*
    private static Player _player;
    public static Player Player
    {
        get
        {
            if (_player == null) FindPlayer();
            return _player;
        }
    }
    */
 
    private static CameraControls _camera;
    public static CameraControls Camera
    {
        get
        {
            if (_camera == null) FindCamera();
            return _camera;
        }
    }

    //public static GameObject[] availableOverworldRooms;

    //public static GameObject[] availableUnderworldRooms;

    public static GameObject[] hit_boxes;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void Start()
    {
        //FindPlayer();
        FindCamera();
    }

    void FixedUpdate()
    {

    }

    /*public static void LoadScene(string sceneName, bool persistPlayer = true)
    {
        if (persistPlayer)
            DontDestroyOnLoad(Player);
        else
        {
            Destroy(Player);
            _player = null;
        }

        Application.LoadLevel(sceneName);

        FindPlayer();
        FindCamera();
    }*/

    private static void FindCamera()
    {
        _camera = FindObjectOfType<CameraControls>();
        if (_camera == null)
        {
            Debug.Log("Cannot find camera on the current scene.");
        }
    }

    /*
    private static void FindPlayer()
    {
        _player = Object.FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.Log("Cannot find player on the current scene.");
        }
    }
    */
 
    public static void PlayerDeath()
    {
        Camera.Shake();
    }

    public static Vector2 getOpenLocation(Parameters.Directions counter_direction, Vector2 starting_location, float max_distance = 1.0f)
    {
        Vector2 new_location = starting_location;
        Vector2 increment = new Vector2(0, 0);
        float increment_distance = 0.1f;
        float current_distance = 0.0f;

        switch (counter_direction)
        {
            case Parameters.Directions.North:
                increment = new Vector2(0, 1) * increment_distance;
                break;
            case Parameters.Directions.NorthEast:
                increment = new Vector2(Mathf.Sin(Mathf.PI / 2), Mathf.Sin(Mathf.PI / 2)) * increment_distance;
                break;
            case Parameters.Directions.East:
                increment = new Vector2(1, 0) * increment_distance;
                break;
            case Parameters.Directions.SouthEast:
                increment = new Vector2(Mathf.Sin(Mathf.PI / 2), Mathf.Sin(3 * Mathf.PI / 2)) * increment_distance;
                break;
            case Parameters.Directions.South:
                increment = new Vector2(0, -1) * increment_distance;
                break;
            case Parameters.Directions.SouthWest:
                increment = new Vector2(Mathf.Sin(3 * Mathf.PI / 2), Mathf.Sin(3 * Mathf.PI / 2)) * increment_distance;
                break;
            case Parameters.Directions.West:
                increment = new Vector2(-1, 0) * increment_distance;
                break;
            case Parameters.Directions.NorthWest:
                increment = new Vector2(Mathf.Sin(3 * Mathf.PI / 2), Mathf.Sin(Mathf.PI / 2)) * increment_distance;
                break;
        }
        while (pointCollides(new_location))
        {
            current_distance += increment_distance;
            new_location += increment;

            if(current_distance < max_distance)
                return starting_location;
        }
        return new_location;
    }

    private static bool pointCollides(Vector2 point)
    {
        return System.Array.Exists(hit_boxes, (GameObject hitbox) => hitbox.GetComponent<Collider2D>().bounds.Contains(point));
    }

    /*
    public static Mobile getTarget()
    {
        Vector2 starting_location = Player.transform.position;
        Mobile[] potentialTargets = Object.FindObjectsOfType<Mobile>();

        Mobile closestTarget = null;
        foreach (Mobile target in potentialTargets)
        {
            if(target != Player)
            {
                float distance = Vector2.Distance(Player.transform.position, target.transform.position);
                if (distance < Player.max_distance)
                    closestTarget = target;
            }
        }

        return closestTarget;
    }
     */
}
