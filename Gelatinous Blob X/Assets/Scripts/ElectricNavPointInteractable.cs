using UnityEngine;
using System.Collections;

public class ElectricNavPointInteractable : Interactable {

    private ElectricNavpoint eNavPoint;

    [SerializeField]
    private RectTransform yesTransformVisual;
    [SerializeField]
    private RectTransform noTransformVisual;

    void Start()
    {
        eNavPoint = this.transform.parent.GetComponent<ElectricNavpoint>();
        if(eNavPoint == null)
        {
            Debug.LogWarning("Cannot find the ElectricNavpoint script on this object! Deleting self.");
            Destroy(this.gameObject);
        }
    }

    public override void OnInteract(Player player)
    {
        if(player.CanTurnToElectricCurrent())
        {
            player.TurnToElectricCurrent(eNavPoint);
        }
    }

    public override RectTransform GetInteractImagePrefab(Player player)
    {
        if(player.CanTurnToElectricCurrent())
        {
            return yesTransformVisual;
        }
        else
        {
            return noTransformVisual;
        }
    }

    public override void OnPlayerEnter(Player player)
    {
        TurnTransparent[] transparentObj = GameObject.FindObjectsOfType<TurnTransparent>();
        foreach (TurnTransparent t in transparentObj)
        {
            t.SetTransparent(true);
        }
        return;
    }

    public override void OnPlayerExit(Player player)
    {
        TurnTransparent[] transparentObj = GameObject.FindObjectsOfType<TurnTransparent>();
        foreach (TurnTransparent t in transparentObj)
        {
            t.SetTransparent(false);
        }
        return;
    }
}
