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

    [Inject]
    private void Init(GameInputReader inputReader)
    {
        this.inputReader = inputReader;
    }
    public override void OnNetworkSpawn()
    {

        if (!IsOwner) return;
        inputReader.InteractEvent += OnTryPickUp;
    }

    private void OnTryPickUp()
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
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent -= OnTryPickUp;
    }
    private void TryPickUp()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen
        Debug.DrawRay(ray.origin, ray.direction * maxPickupDistance, Color.cyan, 1f);

        if (Physics.Raycast(ray, out var hit, maxPickupDistance, ~ghostLayerMask))
        {
            if (hit.collider.TryGetComponent(out PickableObject pickable))
            {
                var objRef = new NetworkObjectReference(pickable.NetworkObject);
                RequestPickUpServerRpc(objRef);
            }
        }
    }

    [ServerRpc]
    private void RequestPickUpServerRpc(NetworkObjectReference pickableRef)
    {
        
        if (pickableRef.TryGet(out var netObj) && netObj.TryGetComponent(out PickableObject pickableObject))
        {
            netObj.TrySetParent(transform, true);
            pickableObject.NotifyObjectPickedClientRpc();
            NotifySetCurrentPickableChangedClientRpc(pickableRef);
        }
    }
    [ClientRpc]
    private void NotifySetCurrentPickableChangedClientRpc(NetworkObjectReference pickableRef)
    {
        if (pickableRef.TryGet(out var netObj) && netObj.TryGetComponent(out PickableObject pickableObject))
        {
            CurrentPickableObject = pickableObject;
            netObj.transform.localPosition = GrabPoint.transform.localPosition;
            netObj.transform.rotation = Quaternion.identity;
        }
    }
    [ClientRpc]
    private void NotifySetCurrentPickableToNullClientRpc()
    {
        CurrentPickableObject = null;
    }


    private void DropObject()
    {
        if (CurrentPickableObject != null)
        {
            var objRef = new NetworkObjectReference(CurrentPickableObject.NetworkObject);
            RequestDropServerRpc(objRef);
            
        }
    }

    [ServerRpc]
    private void RequestDropServerRpc(NetworkObjectReference pickableRef)
    {
        if (pickableRef.TryGet(out var netObj) && netObj.TryGetComponent(out PickableObject pickableObject))
        {
            netObj.TryRemoveParent();
            pickableObject.NotifyObjectDroppedClientRpc();
            NotifySetCurrentPickableToNullClientRpc();
        }
    }
}
