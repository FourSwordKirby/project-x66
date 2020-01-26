using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class CardVisual : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[3];
    public Sprite back;

    public Card card;
    private Image img;
    private Animator anim;

    public bool isHidden;

    private const float REVEAL_TIME = 1.2f;
    private float revealTimer;

    void Awake()
    {
        img = GetComponent<Image>();
        anim = GetComponent<Animator>();
        revealTimer = -1.0f;
    }

    void Update()
    {
        if(revealTimer > 0.0f)
        {
            revealTimer -= Time.deltaTime;
            if(revealTimer <= 0.0f)
            {
                isHidden = false;
            }
        }

        if (isHidden)
        {
            img.sprite = back;
        }
        else
        {
            img.sprite = sprites[(int)card];
        }
    }

    public void Hover()
    {
        anim.SetTrigger("Hover");
    }

    public void Rest()
    {
        anim.SetTrigger("Rest");
    }

    public void Select()
    {
        anim.SetTrigger("Select");
    }

    public void Reveal()
    {
        anim.SetTrigger("Reveal");
        revealTimer = REVEAL_TIME;
    }
}