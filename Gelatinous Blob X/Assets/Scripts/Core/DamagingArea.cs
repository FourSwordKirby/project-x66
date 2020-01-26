using UnityEngine;
using System.Collections;

public class DamagingArea : MonoBehaviour {

    public int DamageOnTouch = 1;
    public float RepelForceStrength = 1.0f;
    public float DamageTicksPerSecond = 1.0f;

    [SerializeField]
    private float timer;

    void Start()
    {
        timer = DamageTicksPerSecond;
    }

    void Update()
    {
        if(timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (timer <= 0.0f)
        {
            Player p;
            if ((p = col.GetComponent<Player>()) != null)
            {
                p.TakeDamage(DamageOnTouch);
                Vector3 toPlayer = p.transform.position - this.transform.position;
                p.GetComponent<Rigidbody>().AddForce(RepelForceStrength * toPlayer.normalized);
                timer = DamageTicksPerSecond;
            }
        }
    }
}
