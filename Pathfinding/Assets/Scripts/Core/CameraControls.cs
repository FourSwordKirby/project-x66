using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraControls : MonoBehaviour {
    /* A bunch of stuff that relates to how the camera shakes*/
    /**
     * Camera "shake" effect preset: shake camera on both the X and Y axes.
     */
    public const uint SHAKE_BOTH_AXES = 0;
    /**
     * Camera "shake" effect preset: shake camera on the X axis only.
     */
    public const uint SHAKE_HORIZONTAL_ONLY = 1;
    /**
     * Camera "shake" effect preset: shake camera on the Y axis only.
     */
    public const uint SHAKE_VERTICAL_ONLY = 2;

	float _fxShakeIntensity = 0.0f;
	float _fxShakeDuration = 0.0f;
	uint _fxShakeDirection = 0;
    Action _fxShakeComplete = null;
	Vector2 _fxShakeOffset = new Vector2();

    float Z_OFFSET = -10;

    /* a bunch of stuff related to zooming in and out */
    public float zoomSpeed = 20f;
    public float maxZoomFOV = 10f;

    public GameObject focalPoint;

    public MonoBehaviour target;
    private Vector3 offset;

    private List<MonoBehaviour> targets_in_view; //accounts for all other targets that aren't just the main focus

    private float original_camera_size;
    private float min_camera_size;
    private float max_camera_size;

	// Use this for initialization
	void Start () {
        //target = null;//GameManager.Player;
        offset = transform.position - target.transform.position;
        Debug.Log("Camera offset: " + offset);
        targets_in_view = new List<MonoBehaviour>();
        original_camera_size = GetComponent<Camera>().orthographicSize;
        min_camera_size = 0.75f * original_camera_size;
        max_camera_size = 2.0f * original_camera_size;
	}
	
	// Update is called once per frame
	void Update () {
        //used to make the camera follow the target in a smooth way
        //if (transform.position != target.transform.position + new Vector3(0, 0, Z_OFFSET))
        //{
        //    float x = ((target.transform.position + new Vector3(0, 0, Z_OFFSET)) - transform.position).x;
        //    float y = ((target.transform.position + new Vector3(0, 0, Z_OFFSET)) - transform.position).y;
        //    GetComponent<Rigidbody2D>().velocity = new Vector2(x * 5.0f, y * 5.0f);
        //}
        //else
        //{
        //    GetComponent<Rigidbody2D>().velocity.Set(0.0f, 0.0f);
        //}
        transform.position = target.transform.position + offset;
        Debug.Log("Target pos: " + target.transform.position + " Expected cam pos: " + transform.position);

        //used to scale the camera's size in response to more targets
        foreach(MonoBehaviour targ in targets_in_view)
        {
            Vector3 camera_position = GetComponent<Camera>().WorldToViewportPoint(targ.transform.position);
            //Debug.Log(camera_position);
            if (!(0.1f < camera_position.x && camera_position.x < 0.9f && 0.1f < camera_position.y && camera_position.y < 0.9f))
            {
                if (GetComponent<Camera>().orthographicSize < max_camera_size)
                {
                    GetComponent<Camera>().orthographicSize = Mathf.MoveTowards(GetComponent<Camera>().orthographicSize, GetComponent<Camera>().orthographicSize * 1.02f, zoomSpeed * Time.deltaTime);
                }
            }

            if ((0.3f < camera_position.x && camera_position.x < 0.7f && 0.3f < camera_position.y && camera_position.y < 0.7f))
            {
                if (GetComponent<Camera>().orthographicSize > min_camera_size)
                {
                    GetComponent<Camera>().orthographicSize = Mathf.MoveTowards(GetComponent<Camera>().orthographicSize, GetComponent<Camera>().orthographicSize * 0.98f, zoomSpeed * Time.deltaTime);
                }
            }
        }

        //Update the "shake" special effect
        if (_fxShakeDuration > 0)
        {
            _fxShakeDuration -= Time.deltaTime;
            if (_fxShakeDuration <= 0)
            {
                _fxShakeOffset.Set(0, 0);
                if (_fxShakeComplete != null)
                    _fxShakeComplete();
            }
            else
            {
                if ((_fxShakeDirection == SHAKE_BOTH_AXES) || (_fxShakeDirection == SHAKE_HORIZONTAL_ONLY))
                    _fxShakeOffset.x = (UnityEngine.Random.Range(-1.0F, 1.0F) * _fxShakeIntensity); //gotta be able to shift the games screen by some percent?;
                if ((_fxShakeDirection == SHAKE_BOTH_AXES) || (_fxShakeDirection == SHAKE_VERTICAL_ONLY))
                    _fxShakeOffset.y = (UnityEngine.Random.Range(-1.0F, 1.0F) * _fxShakeIntensity); //gotta be able to shift the games screen by some percent?;;
            }
        }

        if ((_fxShakeOffset.x != 0) || (_fxShakeOffset.y != 0))
        {
            float x = transform.position.x;
            float y = transform.position.y;
            float z = transform.position.z;

            transform.position = new Vector3(x + _fxShakeOffset.x, y + _fxShakeOffset.y, z);
        }
        //May not need to fix the camera again once we get the camera to follow a player
        /*
        else
        {
            float x = _fxInitialPosition.x;
            float y = _fxInitialPosition.y;
            float z = _fxInitialPosition.z;
            this.transform.position.Set(x, y, z);
        }
        */
	}

    //Used to just follow a specific mobile
    public void Target(Mobile target)
    {
        this.target = target;
    }

    //Used when the player needs to target some enemy etc. Creates a focus point game object
    public void Target(Mobile player, Mobile target)
    {
        GameObject new_target = Instantiate(focalPoint);
        new_target.GetComponent<FocalPoint>().setTargets(player, target);
        this.target = new_target.GetComponent<FocalPoint>();

        //hacky test case stuff
        this.targets_in_view.Add(player);
        this.targets_in_view.Add(target);
    }

    public void Shake(float Intensity = 0.05f, float Duration = 0.5f, Action OnComplete = null, bool Force = true, uint Direction = 0)
    {
        if(!Force && ((_fxShakeOffset.x != 0) || (_fxShakeOffset.y != 0)))
			return;
		_fxShakeIntensity = Intensity;
		_fxShakeDuration = Duration;
        _fxShakeComplete = OnComplete;
		_fxShakeDirection = Direction;
        _fxShakeOffset.Set(0, 0);
    }
}
