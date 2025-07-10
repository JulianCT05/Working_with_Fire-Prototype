using UnityEngine;
using System.Collections.Generic;
using System.Linq; // For .Select and .ToList

public class CraftingGridManager : MonoBehaviour
{
    // Assign your 4 crafting slots here in the Inspector, IN ORDER!
    public List<ItemSlot> craftingSlots;
    public ItemSlot completeItemSlot; // Assign your "result" slot here
    public List<Recipe> recipes; // Assign all your Recipe ScriptableObjects here

    // --- NEW: For Respawning Materials ---
    [Header("Material Respawner")]
    // List of original DraggableItem prefabs you want to be able to respawn
    public List<GameObject> initialMaterialPrefabs;
    // The parent transform where the respawned items should be placed
    public Transform materialSpawnParent;

    // Optional: Keep track of currently active materials if you want to manage duplicates
    // private List<DraggableItem> activeMaterials = new List<DraggableItem>();


    void OnEnable()
    {
        ItemSlot.OnItemDroppedIntoSlot += CheckForRecipe;
    }

    void OnDisable()
    {
        ItemSlot.OnItemDroppedIntoSlot -= CheckForRecipe;
    }

    private void CheckForRecipe(ItemSlot slot, DraggableItem droppedItem)
    {
        Debug.Log("Checking for recipe...");

        List<string> currentItemTypesInGrid = new List<string>();
        foreach (ItemSlot s in craftingSlots)
        {
            if (s.currentItem != null)
            {
                currentItemTypesInGrid.Add(s.currentItem.itemType);
            }
            else
            {
                currentItemTypesInGrid.Add(string.Empty);
            }
        }

        completeItemSlot.ClearSlot(); // Clear any previous result before checking for a new one

        foreach (Recipe recipe in recipes)
        {
            if (recipe.Matches(currentItemTypesInGrid))
            {
                Debug.Log("Recipe Matched: " + recipe.name);
                CraftItem(recipe);
                return;
            }
        }
        Debug.Log("No recipe matched with current items.");
    }

    private void CraftItem(Recipe recipe)
    {
        // Consume the items from the crafting slots
        foreach (ItemSlot s in craftingSlots)
        {
            s.ClearSlot(); // This destroys the item currently in the slot
        }

        // Instantiate the crafted item in the complete slot
        GameObject craftedObject = Instantiate(recipe.craftedItemPrefab, completeItemSlot.transform);
        craftedObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Center it

        Debug.Log("Item Crafted: " + recipe.craftedItemPrefab.name);

        // --- NEW: After crafting, you might want to respawn materials if this is your desired flow ---
        RespawnAllMaterials(); // Uncomment this line if you want materials to respawn immediately after a successful craft
    }

    // --- NEW METHOD: Respawn all original materials ---
    public void RespawnAllMaterials()
    {
        Debug.Log("Respawning all initial materials...");

        // First, clear any draggable items that might still be outside slots or in inventory (optional)
        // You might want to iterate through a specific inventory parent and destroy existing items
        // For simplicity, this example just spawns new ones.

        if (materialSpawnParent == null)
        {
            Debug.LogError("Material Spawn Parent is not assigned! Cannot respawn materials.");
            return;
        }

        // Destroy any existing instances of the initial materials within the spawn parent
        // This prevents infinite duplicates if you call RespawnAllMaterials multiple times.
        foreach (Transform child in materialSpawnParent)
        {
            DraggableItem existingItem = child.GetComponent<DraggableItem>();
            if (existingItem != null && initialMaterialPrefabs.Any(prefab => prefab.GetComponent<DraggableItem>()?.itemType == existingItem.itemType))
            {
                Destroy(child.gameObject);
            }
        }


        // Instantiate each initial material prefab
        foreach (GameObject prefab in initialMaterialPrefabs)
        {
            GameObject newMaterial = Instantiate(prefab, materialSpawnParent);
            // Optionally set position within the spawn parent, e.g., in a grid
            // For now, they'll just stack or appear based on the layout of materialSpawnParent
            RectTransform rt = newMaterial.GetComponent<RectTransform>();
            if (rt != null)
            {
                // You might want to arrange them in a grid here
                // For example, using a HorizontalLayoutGroup or VerticalLayoutGroup on materialSpawnParent
                rt.anchoredPosition = Vector2.zero; // Or some calculated position
            }
            Debug.Log($"Respawned: {prefab.name}");
        }
    }

    // Optional: A public method to trigger crafting manually (e.g., with a "Craft" button)
    public void TryCraft()
    {
        Debug.Log("Attempting to craft via button.");
        List<string> currentItemTypesInGrid = new List<string>();
        foreach (ItemSlot s in craftingSlots)
        {
            if (s.currentItem != null)
            {
                currentItemTypesInGrid.Add(s.currentItem.itemType);
            }
            else
            {
                currentItemTypesInGrid.Add(string.Empty);
            }
        }

        completeItemSlot.ClearSlot();

        foreach (Recipe recipe in recipes)
        {
            if (recipe.Matches(currentItemTypesInGrid))
            {
                Debug.Log("Recipe Matched: " + recipe.name);
                CraftItem(recipe);
                return;
            }
        }
        Debug.Log("No recipe matched for manual craft attempt.");
    }
}