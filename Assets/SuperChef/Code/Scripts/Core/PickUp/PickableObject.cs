using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject))]
public class PickableObject : NetworkBehaviour
{
    private Rigidbody rb;
    private Collider col;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>(); 
    }
    [ClientRpc]
    public void NotifyObjectPickedClientRpc()
    {
        rb.isKinematic = true;
        col.enabled = false;
    }
    [ClientRpc]
    public void NotifyObjectDroppedClientRpc()
    {
        rb.isKinematic = false;
        col.enabled = true;
    }

}
