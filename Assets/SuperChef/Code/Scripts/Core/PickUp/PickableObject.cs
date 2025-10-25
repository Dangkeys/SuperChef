using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

[RequireComponent(typeof(AutoInjectOnAwake), typeof(Rigidbody), typeof(Collider))]
public class PickableObject : NetworkBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private NetworkObject netObj;

    private void Awake()
    {
        netObj = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void SetPickUpState(bool isPickedUp, Transform grabPointPosition = null, Transform parent = null)
    {
        Vector3 targetPos = grabPointPosition != null ? grabPointPosition.position : transform.position;

        if (IsServer)
        {
            NetworkObject parentNetObj = (isPickedUp && parent != null) ? parent.GetComponent<NetworkObject>() : null;
            SetParentNetworkObject(parentNetObj, isPickedUp, targetPos);
        }
        else
        {
            NetworkObjectReference parentRef = default;
            if (isPickedUp && parent != null)
            {
                NetworkObject parentNetObj = parent.GetComponent<NetworkObject>();
                if (parentNetObj != null)
                    parentRef = new NetworkObjectReference(parentNetObj);
            }
            SetParentNetworkObjectServerRpc(parentRef, isPickedUp, targetPos);
        }
    }

    private void TogglePhysics(bool isPickedUp)
    {
        rb.isKinematic = isPickedUp;
        // col.isTrigger = isPickedUp;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetParentNetworkObjectServerRpc(NetworkObjectReference parentNetworkObjectReference, bool isPickedUp, Vector3 grabPointPosition)
    {
        NetworkObject parentNetworkObject = null;
        parentNetworkObjectReference.TryGet(out parentNetworkObject);
        SetParentNetworkObject(parentNetworkObject, isPickedUp, grabPointPosition);
    }

    private void SetParentNetworkObject(NetworkObject parentNetworkObject, bool isPickedUp = true, Vector3 grabPointPosition = default)
    {
        Transform parentTransform = parentNetworkObject != null ? parentNetworkObject.transform : null;
        netObj.TrySetParent(parentTransform);
        if (isPickedUp)
        {
            transform.position = grabPointPosition;
            transform.localRotation = Quaternion.identity;
        }
        TogglePhysics(isPickedUp);
    }
}
