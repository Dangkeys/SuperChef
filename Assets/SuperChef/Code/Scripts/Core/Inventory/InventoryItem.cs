using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkRigidbody))]
public class InventoryItem : NetworkBehaviour
{
    [field: SerializeField] public InventoryItemSO InventoryItemSO { get; private set; }
    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void SetIsKinematic(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }
}
