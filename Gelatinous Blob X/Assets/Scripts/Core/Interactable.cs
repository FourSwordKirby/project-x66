using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    public abstract void OnPlayerEnter(Player player);
    public abstract void OnInteract(Player player);
    public abstract void OnPlayerExit(Player player);

    /// <summary>
    /// Retrieves a visual that represents this Interactable in the InteractionBox.
    /// Returns null if this Interactable has not chosen a visual.
    /// </summary>
    public virtual RectTransform GetInteractImagePrefab(Player player)
    {
        return null;
    }
}
