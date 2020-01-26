using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActivationEvent : MonoBehaviour {

    private Player player;
    private PlayerCamera playerCamera;

    public Camera cutsceneCamera;
    [SerializeField]
    public List<Enemy> enemies;
    public List<GameObject> targetPositions;
    public SecurityDoor roomDoor;

    public DialogEvent dialogEvent;

    void Start()
    {
        player = GameManager.Player;
        playerCamera = FindObjectOfType<PlayerCamera>();
        foreach (Enemy enemy in enemies)
        {
            enemy.enabled = false;
            enemy.targetingCone.SetActive(false);
            enemy.dangerHitbox.SetActive(false);
        }
        cutsceneCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if(targetPositions.Count >= enemies.Count)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];
                GameObject target = targetPositions[i];

                if (target != null && enemy.GetComponent<BoxCollider>().bounds.Contains(target.transform.position))
                {
                    enemy.enabled = false;
                }
            }
        }

        //The event is over when the dialog has been exhausted
        if (dialogEvent == null)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.enabled = true;
                enemy.targetingCone.SetActive(true);
            }
            playerCamera.gameObject.SetActive(true);
            cutsceneCamera.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Player>() != null)
        {
            TurnTransparent[] transparentObj = GameObject.FindObjectsOfType<TurnTransparent>();
            foreach (TurnTransparent t in transparentObj)
            {
                t.SetTransparent(false);
            }

            cutsceneCamera.gameObject.SetActive(true);
            playerCamera.gameObject.SetActive(false);

            Camera.SetupCurrent(cutsceneCamera);
            foreach (Enemy enemy in enemies)
            {
                enemy.enabled = true;
            }

            roomDoor.Unlock();
        }
    }

    private IEnumerator Pause(float duration)
    {
        yield return new WaitForSeconds(duration);
    }
}
