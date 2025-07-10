using UnityEngine;
using System.Collections.Generic;
using System.Linq; // For LINQ operations like Count, All, SequenceEqual

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
    // The recipe grid. Use "null" or string.Empty for empty slots.
    // The size of this list must match your crafting grid size (e.g., 4 for 2x2, 9 for 3x3).
    public List<string> requiredItemTypesGrid; // Changed from requiredItemTypes

    public GameObject craftedItemPrefab; // The prefab to spawn when this recipe is crafted

    // --- NEW: Optional for display purposes, helps visualize the recipe in editor ---
    [Tooltip("For editor visualization only. Number of columns in your crafting grid.")]
    public int gridColumns = 2; // Set this to 2 for a 2x2 grid, 3 for 3x3 etc.

    // This method will now check for exact positional matching, including empty slots.
    public bool Matches(List<string> itemTypesInSlots)
    {
        if (itemTypesInSlots == null || itemTypesInSlots.Count != requiredItemTypesGrid.Count)
        {
            Debug.Log($"Recipe.Matches: Mismatch in slot count. In slots: {itemTypesInSlots?.Count ?? 0}, Required: {requiredItemTypesGrid.Count}");
            return false;
        }

        // Compare each slot directly.
        for (int i = 0; i < requiredItemTypesGrid.Count; i++)
        {
            string requiredType = requiredItemTypesGrid[i];
            string actualType = itemTypesInSlots[i];

            // If requiredType is null/empty, actualType must also be null/empty
            if (string.IsNullOrEmpty(requiredType))
            {
                if (!string.IsNullOrEmpty(actualType))
                {
                    Debug.Log($"Recipe.Matches: Slot {i} expected empty, found '{actualType}'");
                    return false;
                }
            }
            // If requiredType is not null/empty, actualType must match exactly
            else
            {
                if (requiredType != actualType)
                {
                    Debug.Log($"Recipe.Matches: Slot {i} expected '{requiredType}', found '{actualType}'");
                    return false;
                }
            }
        }

        Debug.Log($"Recipe.Matches: All slots matched for recipe {this.name}.");
        return true;
    }
}