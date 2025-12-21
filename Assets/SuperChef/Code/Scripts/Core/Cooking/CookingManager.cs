using Unity.Netcode;
using UnityEngine;
using Zenject;

public class CookingManager : NetworkBehaviour
{
    private HoldableProvider holdableProvider;
    private PickUp pickUp;
    private float maxInteractDistance = 10f;
    [Inject]
    private void Init(HoldableProvider holdableProvider, PickUp pickUp)
    {
        this.holdableProvider = holdableProvider;
        this.pickUp = pickUp;
    }

    public void PerformInteraction()
    {
        if (pickUp.CurrentPickableObject != null) return;
        if (!Camera.main) return;

        if (!RaycastHelper.TryGetComponentFromCenterRaycast<ChoppingHandler>(
            maxInteractDistance, out var choppingHandler, out _)) return;
        if (holdableProvider.CurrentHoldableItemSO == null) return;
        if (holdableProvider.CurrentHoldableItemSO.Name != "Knife") return;//for test purposes
        choppingHandler.RequestToCutServerRpc();
    }

}