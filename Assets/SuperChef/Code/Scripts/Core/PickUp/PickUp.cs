using System;
using Mono.CSharp;
using NUnit.Framework.Internal;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class PickUp : NetworkBehaviour
{
    private GameInputReader inputReader;
    public PickableObject CurrentPickableObject { get; private set; }
    [field: SerializeField] public Transform GrabPoint { get; private set; }

    [SerializeField] private float maxPickupDistance = 10f;
    [SerializeField] private LayerMask ghostLayerMask;
    public Action OnCurrentPickableObjectChanged;

    [Inject]
    private void Init(GameInputReader inputReader)
    {
        this.inputReader = inputReader;
    }


    private void OnInteract()
    {
        if (CurrentPickableObject == null)
        {
            TryPickUp();
        }
        else
        {
            DropObject();
        }
    }
    public bool PerformInteraction()
    {
        // If we are holding the plate, drop it and STOP.
        if (CurrentPickableObject != null)
        {
            DropObject();
            return true;
        }

        // Check if we are looking at a physical object to pick up
        return TryPickUp();
    }
    private bool TryPickUp()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out var hit, maxPickupDistance, ~ghostLayerMask))
        {
            if (hit.collider.TryGetComponent(out PickableObject pickable))
            {
                var objRef = new NetworkObjectReference(pickable.NetworkObject);
                RequestPickUpServerRpc(objRef);
                return true;
            }
        }
        return false;
    }

    [Rpc(SendTo.Server)]
    private void RequestPickUpServerRpc(NetworkObjectReference pickableRef)
    {

        if (pickableRef.TryGet(out var netObj) && netObj.TryGetComponent(out PickableObject pickableObject))
        {
            netObj.TrySetParent(transform, true);
            pickableObject.SetIsKinematic(true);
            pickableObject.NotifyObjectPickedClientRpc();
            NotifySetCurrentPickableChangedClientRpc(pickableRef);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifySetCurrentPickableChangedClientRpc(NetworkObjectReference pickableRef)
    {
        if (pickableRef.TryGet(out var netObj) && netObj.TryGetComponent(out PickableObject pickableObject))
        {
            CurrentPickableObject = pickableObject;
            netObj.transform.localPosition = GrabPoint.transform.localPosition;
            netObj.transform.localRotation = Quaternion.identity;

            OnCurrentPickableObjectChanged?.Invoke();
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifySetCurrentPickableToNullClientRpc()
    {
        CurrentPickableObject = null;
        OnCurrentPickableObjectChanged?.Invoke();
    }


    private void DropObject()
    {
        if (CurrentPickableObject != null)
        {
            var objRef = new NetworkObjectReference(CurrentPickableObject.NetworkObject);
            RequestDropServerRpc(objRef);

        }
    }

    [Rpc(SendTo.Server)]
    private void RequestDropServerRpc(NetworkObjectReference pickableRef)
    {
        if (pickableRef.TryGet(out var netObj) && netObj.TryGetComponent(out PickableObject pickableObject))
        {
            netObj.TryRemoveParent();
            pickableObject.SetIsKinematic(false);
            pickableObject.NotifyObjectDroppedClientRpc();
            NotifySetCurrentPickableToNullClientRpc();
        }
    }
}
