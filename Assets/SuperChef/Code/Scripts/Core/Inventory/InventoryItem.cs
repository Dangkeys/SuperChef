using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [field: SerializeField] public InventoryItemSO inventoryItemSO { get; private set; }
}
