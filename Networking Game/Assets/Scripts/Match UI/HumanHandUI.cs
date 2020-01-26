using UnityEngine;
using System.Collections;

public class HumanHandUI : HandUI {

    public RpsHumanPlayer humanPlayer;

    public CardVisual cardWithControlPrefab;

    void OnEnable()
    {
        this.isInteractable = true;
    }

    public void Initialize(RpsHumanPlayer player)
    {
        humanPlayer = player;
        this.player = player;
        this.isHidden = false;
    }

    public override void CreateHand()
    {
        for (int i = 0; i < player.CardCount(); ++i)
        {
            CardVisual c = Instantiate<CardVisual>(cardWithControlPrefab);
            c.transform.SetParent(this.transform, false);
            c.transform.SetSiblingIndex(i);
            c.card = player.CardAt(i);
            CardControl cc = c.GetComponent<CardControl>();
            cc.parentHand = this;
            cardVisuals.Add(c);
        }
        lastHandCount = cardVisuals.Count;
    }

    public override void LoadForMatch(RpsAgent player)
    {
        base.LoadForMatch(player);
        if(!(player is RpsHumanPlayer))
        {
            Debug.LogError("HumanHandUI loading a non-human player!");
            return;
        }
        isInteractable = true;
    }

    public override void Hide()
    {
        ClearHand();
        this.gameObject.SetActive(false);
    }

    public void PointerEnterOnCardIndex(int index)
    {
        CurrentCard = index;
        UpdateHoverAnimation();
    }


    public void PointerClickOnCardIndex(int index)
    {
        if (CurrentCard != index)
        {
            CurrentCard = index;
            UpdateHoverAnimation();
        }
        else
        {
            PlaySelectionAnimation();
            isInteractable = false;
            humanPlayer.CmdSelectCard(index);
        }
    }
}
