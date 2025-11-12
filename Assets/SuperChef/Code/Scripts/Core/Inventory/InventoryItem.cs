using Unity.Netcode;
using UnityEngine;

public class InventoryItem : NetworkBehaviour
{
    [field: SerializeField] public InventoryItemSO InventoryItemSO { get; private set; }
}
