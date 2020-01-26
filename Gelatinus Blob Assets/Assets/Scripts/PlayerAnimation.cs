using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {

    public Player player;
    public Transform eyes;
    public Transform body;

    public AnimationCurve idle;
    public float idleFrequency;
    public AnimationCurve moving;
    public float movingFrequency;

    private float height;
    private float targetHeight;

    private float startHeight;
    private Vector3 eyeStartHeight;

    void Start() {
        height = startHeight = idle.Evaluate(0);
        eyeStartHeight = eyes.position;
    }

    void Update() {
        switch (player.state) {
            case PlayerState.IDLE:
                targetHeight = idle.Evaluate(Time.time * idleFrequency);
                break;
            case PlayerState.MOVING:
                targetHeight = moving.Evaluate(Time.time * movingFrequency);
                break;
        }

        height = Mathf.Lerp(height, targetHeight, Time.deltaTime * 10);
        transform.localScale = new Vector3(1, height, 1);

        eyes.localPosition = eyeStartHeight + Vector3.up * (height - startHeight) / 2;
        eyes.localScale = new Vector3(1, height * (height + 0.1f), 1);

        body.localScale = new Vector3(0.5f + 0.5f/height, 1, 1);
    }
}
