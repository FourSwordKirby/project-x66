using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractionBox : MonoBehaviour {

    public const float OPEN_ANIMATION_TIME = 0.3f;

    public RectTransform DefaultImagePrefab;

    private Player player;
    private bool isVisible;
    private RectTransform currentImagePrefab;
    private RectTransform currentImage;

    // Self references
    private RectTransform imageFrame;
    private Animator animator;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

	// Use this for initialization
	void Start ()
    {
        player = GameManager.Player;
        imageFrame = this.transform.FindChild("ReferenceRectangle").GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!player.enabled)
        {
            if(isVisible)
            {
                CloseInteractionBox();
            }
            return;
        }

        if (player.IsTouchingInteractable)
        {
            if (!isVisible)
            {
                // Play opening animation
                animator.SetTrigger("Open");

                if (player.CurrentInteractable.GetInteractImagePrefab(player) != null)
                {
                    currentImagePrefab = player.CurrentInteractable.GetInteractImagePrefab(player);
                }
                else
                {
                    currentImagePrefab = DefaultImagePrefab;
                }
                SpawnImageInFrame(currentImagePrefab);
                isVisible = true;
            }
            else
            {
                RectTransform targetImagePrefab;
                if (player.CurrentInteractable.GetInteractImagePrefab(player) != null)
                {
                    targetImagePrefab = player.CurrentInteractable.GetInteractImagePrefab(player);
                }
                else
                {
                    targetImagePrefab = DefaultImagePrefab;
                }
                if(currentImagePrefab != targetImagePrefab)
                {
                    Debug.Log("Changing image");
                    DestoryCurrentImage();
                    currentImagePrefab = targetImagePrefab;
                    SpawnImageInFrame(currentImagePrefab);
                }
            }
        }
        else
        {
            if (isVisible)
            {
                CloseInteractionBox();
            }
        }
	}

    private void CloseInteractionBox()
    {                
        // Play closing animation
        animator.SetTrigger("Close");

        currentImagePrefab = null;
        DestoryCurrentImage();
        isVisible = false;
    }

    private IEnumerator ShowInteractionImage()
    {
        yield return new WaitForSeconds(OPEN_ANIMATION_TIME);
        SpawnImageInFrame(currentImagePrefab);
    }

    private void SpawnImageInFrame(RectTransform imagePrefab)
    {
        currentImage = GameObject.Instantiate<RectTransform>(imagePrefab);
        currentImage.SetParent(this.imageFrame, false);
    }

    private void DestoryCurrentImage()
    {
        if(currentImage != null)
        {
            Destroy(currentImage.gameObject);
        }
        currentImage = null;
    }
}
