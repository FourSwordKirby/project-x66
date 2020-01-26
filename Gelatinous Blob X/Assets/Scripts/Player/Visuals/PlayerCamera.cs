using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerCamera : MonoBehaviour {
    /* A bunch of stuff that relates to how the camera shakes*/
    public const uint SHAKE_BOTH_AXES = 0;
    public const uint SHAKE_HORIZONTAL_ONLY = 1;
    public const uint SHAKE_VERTICAL_ONLY = 2;

    float _fxShakeIntensity = 0.0f;
    float _fxShakeDuration = 0.0f;
    uint _fxShakeDirection = 0;
    Action _fxShakeComplete = null;
    Vector2 _fxShakeOffset = new Vector2();

    //Stuff relating to causing flashes to appear on screen
    public Image flashPanel;
    private bool flash = false;
    private float flashFadeDuration = 1.0f;

    public GameObject player;
    private Vector3 offset;

    void Start()
    {
        if(flashPanel == null)
        {
            flashPanel = this.GetComponentInChildren<Image>();
            if(flashPanel == null)
            {
                Debug.LogWarning("Cannot find flash panel!");
            }
        }
        player = GameManager.Player.gameObject;
        offset = transform.position - player.transform.position;
    }

    void Update() {
        transform.position = player.transform.position + offset;

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

        //Stuff to imitate a flash
        if (flash)
        {
            float newAlpha = flashPanel.color.a - (Time.deltaTime / flashFadeDuration);
            if (newAlpha <= 0)
            {
                newAlpha = 0;
                flash = false;
            }
            Color previousColor = flashPanel.color;
            flashPanel.color = new Color(previousColor.a, previousColor.g, previousColor.b, newAlpha);
        }
    }


    public void Shake(float Intensity = 0.05f, float Duration = 0.5f, Action OnComplete = null, bool Force = true, uint Direction = 0)
    {
        if (!Force && ((_fxShakeOffset.x != 0) || (_fxShakeOffset.y != 0)))
            return;
        _fxShakeIntensity = Intensity;
        _fxShakeDuration = Duration;
        _fxShakeComplete = OnComplete;
        _fxShakeDirection = Direction;
        _fxShakeOffset.Set(0, 0);
    }

    public void Flash(Color color, float fadeTime = 1.0f)
    {
        flash = true;
        flashPanel.color = color;
        flashPanel.enabled = true;
        flashFadeDuration = fadeTime;
    }
}
