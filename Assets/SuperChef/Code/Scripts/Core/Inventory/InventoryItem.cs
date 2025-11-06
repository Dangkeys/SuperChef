using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [field: SerializeField] public InventoryItemSO InventoryItemSO { get; private set; }
}
