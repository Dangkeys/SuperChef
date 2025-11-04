using System;
using Mono.CSharp;
using Unity.Netcode;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(AutoInjectOnAwake))]
public class PickUp : NetworkBehaviour
{
    private GameInputReader _inputReader;
    public PickableObject CurrentPickableObject { get; private set; }
    [field: SerializeField] public Transform GrabPoint { get; private set; }

    [SerializeField] private float maxPickupDistance = 10f;
    
    [Inject]
    private void Init(GameInputReader inputReader)
    {
        _inputReader = inputReader;

    }
    public override void OnNetworkSpawn()
    {

        if (!IsOwner) return;
        _inputReader.InteractEvent += OnTryPickUp;
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
        _inputReader.InteractEvent -= OnTryPickUp;
    }
    private void TryPickUp()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen
        Debug.DrawRay(ray.origin, ray.direction * maxPickupDistance, Color.cyan, 1f);

        if (Physics.Raycast(ray, out var hit, maxPickupDistance))
        {
            if (hit.collider.TryGetComponent(out PickableObject pickable))
            {
                // send pickup request to server
                var objRef = new NetworkObjectReference(pickable.NetworkObject);
                RequestPickUpServerRpc(objRef);
            }
        }
    }

    [ServerRpc]
    private void RequestPickUpServerRpc(NetworkObjectReference pickableRef)
    {
        if (pickableRef.TryGet(out var netObj) && netObj.TryGetComponent(out PickableObject pickable))
        {
            pickable.SetPickUpParent(this);
        }
    }

    private void DropObject()
    {
        if (CurrentPickableObject != null)
        {
            var objRef = new NetworkObjectReference(CurrentPickableObject.NetworkObject);
            RequestDropServerRpc(objRef);
            CurrentPickableObject = null;
        }
    }

    [ServerRpc]
    private void RequestDropServerRpc(NetworkObjectReference pickableRef)
    {
        if (pickableRef.TryGet(out var netObj) && netObj.TryGetComponent(out PickableObject pickable))
        {
            pickable.ClearPickUpParent();
        }
    }

    // called from PickableObject after pickup
    public void SetCurrentPickable(PickableObject obj)
    {
        CurrentPickableObject = obj;
    }
}
