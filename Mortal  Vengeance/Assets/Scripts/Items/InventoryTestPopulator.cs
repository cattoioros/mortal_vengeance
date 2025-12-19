using UnityEngine;

public class InventoryTestPopulator : MonoBehaviour
{
    public InventoryUIManager uiManager;
    public ItemData testItem;

    void Start()
    {
        uiManager.DebugAddItemToFirstSlot(testItem);
    }
}
