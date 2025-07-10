using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Required for Image if your items are UI Images

public class DraggableItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // Assign your Canvas here in the Inspector. This is crucial for correct scaling.
    [SerializeField] private Canvas canvas;

    // This string will define what the item IS for crafting recipes.
    // Example: "RedDye", "BlueDye", "Paper", "Wood", "IronIngot"
    public string itemType;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 originalPosition; // Stores the item's position before dragging
    private Transform originalParent; // Stores the item's parent before dragging

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Ensure a CanvasGroup is present, otherwise dragging won't work well
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Try to find the Canvas if not assigned, useful for quick setup
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindObjectOfType<Canvas>();
                Debug.LogWarning("DraggableItem: Canvas not assigned. Automatically found Canvas: " + (canvas != null ? canvas.name : "None"));
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // This is called when the mouse button is pressed down on the item.
        // Useful if you want a visual feedback or sound effect on click.
        Debug.Log("OnPointerDown: " + gameObject.name);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag: " + gameObject.name + " (" + itemType + ")");

        // Store original parent and position
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        // Temporarily reparent to the Canvas to ensure it renders on top of everything
        // This is important if your inventory slots are nested deep in the UI hierarchy
        transform.SetParent(canvas.transform);

        // Make the item semi-transparent and ignore raycasts while dragging
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the item based on pointer movement and canvas scaling
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag: " + gameObject.name + " (" + itemType + ")");

        // Restore full opacity and enable raycasts
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Important: If the item was *not* dropped on a valid `ItemSlot` (which handles re-parenting),
        // it will still have the Canvas as its parent. In this case, return it to its original spot.
        // The `ItemSlot.OnDrop` method will handle successful drops by setting a new parent.
        if (transform.parent == canvas.transform)
        {
            ResetToOriginalState();
        }
    }

    // New method: To explicitly reset the item's position and parent.
    // Useful if the item is "returned" to inventory or consumed in crafting.
    public void ResetToOriginalState()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
        Debug.Log("DraggableItem: " + gameObject.name + " reset to original parent: " + originalParent.name);
    }
}