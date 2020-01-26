using UnityEngine;
using System.Collections.Generic;

public class HandUI : MonoBehaviour {

    public RpsAgent player;

    protected int lastHandCount;
    protected int lastHoveredCard;
    protected List<CardVisual> cardVisuals;
    public CardVisual cardPrefab;

    public int CurrentCard;
    public bool isHidden;
    public bool isInteractable;

    public Direction dir;

    void Awake()
    {
        cardVisuals = new List<CardVisual>();
        lastHandCount = -1;
        lastHoveredCard = -1;
        isInteractable = false;
    }

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update () {
        if(player == null)
        {
            if(cardVisuals.Count > 0)
            {
                ClearHand();
            }
            return;
        }

        if(isInteractable && player.hasSelectedCardForMatch)
        {
            PlaySelectionAnimation();
            isInteractable = false;
        }
	}

    public virtual void LoadForMatch(RpsAgent player)
    {
        this.player = player;
        isInteractable = true;
        this.gameObject.SetActive(true);

        ClearHand();
        CreateHand();
        RepositionHand();
        if (!isHidden)
        {
            cardVisuals[CurrentCard].Hover();
            lastHoveredCard = 0;
        }
        else
        {
            CurrentCard = player.CardCount() / 2;
            lastHoveredCard = CurrentCard;
        }
    }

    public virtual void Hide()
    {
        this.player = null;
        ClearHand();
        this.gameObject.SetActive(false);
    }

    public void IncrementCurrentCard()
    {
        CurrentCard = (CurrentCard + 1) % player.CardCount();
        if (CurrentCard >= player.CardCount())
        {
            CurrentCard -= player.CardCount();
        }
        UpdateHoverAnimation();
    }

    public void DecrementCurrentCard()
    {
        CurrentCard = (CurrentCard - 1) % player.CardCount();
        if (CurrentCard < 0)
        {
            CurrentCard += player.CardCount();
        }
        UpdateHoverAnimation();
    }


    public void PlaySelectionAnimation()
    {
        if (isInteractable)
        {
            cardVisuals[CurrentCard].Select();
        }
    }

    public CardVisual GetCurrentCardVisual()
    {
        return cardVisuals[CurrentCard];
    }

    public void UpdateHoverAnimation()
    {
        if (isInteractable && !isHidden && lastHoveredCard != CurrentCard)
        {
            cardVisuals[lastHoveredCard].Rest();
            cardVisuals[CurrentCard].Hover();
            lastHoveredCard = CurrentCard;
        }
    }

    public void ClearHand()
    {
        foreach (CardVisual cv in cardVisuals)
        {
            Destroy(cv.gameObject);
        }
        cardVisuals.Clear();
    }

    public virtual void CreateHand()
    {
        ClearHand();
        for (int i = 0; i < player.CardCount(); ++i)
        {
            CardVisual c = Instantiate<CardVisual>(cardPrefab);
            c.transform.SetParent(this.transform, false);
            c.transform.SetSiblingIndex(i);
            c.card = player.CardAt(i);
            c.isHidden = isHidden;
            cardVisuals.Add(c);
        }
        lastHandCount = cardVisuals.Count;
    }

    public void RepositionHand()
    {
        RectTransform rect = this.GetComponent<RectTransform>();
        float width = rect.rect.width;
        float spacing = width / player.CardCount();
        float startX = -width / 2 + spacing / 2;
        for (int i = 0; i < player.CardCount(); ++i)
        {
            CardVisual c = cardVisuals[i];
            c.GetComponent<RectTransform>().anchoredPosition = new Vector3(startX + i * spacing, 0.0f, 0.0f);
        }
        CurrentCard = 0;
    }

    public void SetAndRevealCard(Card opponentsCard)
    {
        if(CurrentCard < cardVisuals.Count)
        {
            cardVisuals[CurrentCard].card = opponentsCard;
            cardVisuals[CurrentCard].Reveal();
        }
    }

}
