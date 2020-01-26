using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IDragHandler
{
    private RectTransform selfTransform;

    void Awake()
    {
        selfTransform = this.GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        selfTransform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y);
    }
}
