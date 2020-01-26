using UnityEngine;
using UnityEngine.EventSystems;

public class CardControl : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public HumanHandUI parentHand;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (parentHand != null)
        {
            parentHand.PointerEnterOnCardIndex(this.transform.GetSiblingIndex());
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        parentHand.PointerClickOnCardIndex(this.transform.GetSiblingIndex());
    }
}
