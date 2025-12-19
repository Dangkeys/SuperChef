using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuildableObject : InventoryItem
{
    private Renderer[] renderers;
    private Material originalMaterial;
    private Rigidbody rb;
    [field: SerializeField] public LayerMask ActiveLayerMask { get; private set; }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();
    }
    [ClientRpc]
    public void NotifyBuildingObjectPlacedClientRpc()
    {
        rb.isKinematic = true;
    }

}
