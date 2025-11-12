using Unity.Netcode;
using UnityEngine;

public class BuildableObject : NetworkBehaviour
{
    private Renderer[] renderers;
    private Material originalMaterial;
    [field:SerializeField] public LayerMask  ActiveLayerMask { get; private set; }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

}
