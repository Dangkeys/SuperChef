using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject), typeof(FollowTransform))]
public class PickableObject : NetworkBehaviour
{
    private Rigidbody rb;
    private FollowTransform followTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        followTransform = GetComponent<FollowTransform>();
    }

    public void SetPickUpParent(PickUp pickUp)
    {
        if (!IsServer) return;

        rb.isKinematic = true; // stop physics
        followTransform.enabled = true;

        // tell everyone to attach it visually
        var pickupRef = new NetworkObjectReference(pickUp.NetworkObject);
        SetPickUpParentClientRpc(pickupRef);
    }

    [ClientRpc]
    private void SetPickUpParentClientRpc(NetworkObjectReference pickUpRef)
    {
        if (pickUpRef.TryGet(out var pickupObj) && pickupObj.TryGetComponent(out PickUp pickUp))
        {
            followTransform.SetTargetTransform(pickUp.GrabPoint);
            pickUp.SetCurrentPickable(this);
        }
    }

    public void ClearPickUpParent()
    {
        if (!IsServer) return;

        rb.isKinematic = false;
        followTransform.enabled = false;

        ClearPickUpParentClientRpc();
    }

    [ClientRpc]
    private void ClearPickUpParentClientRpc()
    {
        followTransform.SetTargetTransform(null);
    }
}
