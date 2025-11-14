using Unity.Netcode;
using UnityEngine;

public class BuildableObject : InventoryItem
{
    private Renderer[] renderers;
    private Material originalMaterial;
    private Rigidbody rb;
    [field:SerializeField] public LayerMask  ActiveLayerMask { get; private set; }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

}
