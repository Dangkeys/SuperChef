using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkRigidbody), typeof(NetworkObject))]
public class PickableObject : NetworkBehaviour
{
    private Rigidbody rb;
    private Collider col;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void NotifyObjectPickedClientRpc()
    {
        col.enabled = false;
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void NotifyObjectDroppedClientRpc()
    {
        col.enabled = true;
    }
    public void SetIsKinematic(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }
}
