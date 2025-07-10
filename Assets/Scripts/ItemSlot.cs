using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI; // Required for Image component

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public DraggableItem currentItem { get; private set; } // Reference to the item currently in the slot

    // Event to notify the CraftingGridManager when an item is dropped
    public delegate void OnItemDropped(ItemSlot slot, DraggableItem droppedItem);
    public static event OnItemDropped OnItemDroppedIntoSlot;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop in slot: " + gameObject.name);

        DraggableItem droppedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (droppedItem != null)
        {
            // If there's already an item in this slot, return it to its original position
            // You might want a more sophisticated swap logic here
            if (currentItem != null)
            {
                currentItem.ResetToOriginalState(); // This assumes ResetToOriginalState exists and correctly moves the item back
            }

            // Set the new item
            currentItem = droppedItem;
            RectTransform droppedRect = droppedItem.GetComponent<RectTransform>();

            // Re-parent it to this slot
            droppedRect.SetParent(transform);

            // Reset local position so it centers in the slot
            droppedRect.anchoredPosition = Vector2.zero;

            // Notify listeners (the CraftingGridManager)
            OnItemDroppedIntoSlot?.Invoke(this, droppedItem);
        }
    }

    // Call this when an item needs to be removed from the slot (e.g., consumed in crafting)
    public void ClearSlot()
    {
        if (currentItem != null)
        {
            Destroy(currentItem.gameObject); // Or pool it, or return to inventory
            currentItem = null;
        }
    }
}