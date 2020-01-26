using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class ChallengeLobbyPlayerUI : MonoBehaviour, IPointerClickHandler {

    private RpsAgent player;

    public Sprite defaultSprite;
    public Sprite inMatchSprite;
    public Sprite lostSprite;
    public Sprite winSprite;

    public bool isAIPlayer;

    private SpriteRenderer render;
    private TextMesh nameTag;

    private Collider2D myCollider;

    void Awake()
    {
        myCollider = this.GetComponent<Collider2D>();
    }

	// Use this for initialization
	void Start () {
        player = transform.parent.GetComponent<RpsAgent>();
        this.gameObject.transform.position = player.challegeLobbyPosition;

        this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        this.render = this.GetComponent<SpriteRenderer>();
        this.nameTag = this.GetComponentInChildren<TextMesh>();
        nameTag.text = player.AgentName;
	}
	
	// Update is called once per frame
	void Update () {
        if (player.CardCount() == 0 && player.stars > 0)
            render.sprite = winSprite;
        else if (player.stars == 0)
            render.sprite = lostSprite;
        else if (player.IsInMatch)
            render.sprite = inMatchSprite;
        else if (!player.IsInMatch)
            render.sprite = defaultSprite;
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (player.isLocalPlayer)
        {
            return;
        }
        PlayerInfoUI playerInfo = GameObject.FindObjectOfType<PlayerInfoUI>();
        playerInfo.loadInfo(player, isAIPlayer);
    }
}
