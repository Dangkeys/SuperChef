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
        
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (!Physics.Raycast(ray, out var hit, maxInteractDistance)) return;
        if (!hit.collider.TryGetComponent(out ChoppingHandler choppingHandler)) return;
        if(holdableProvider.CurrentHoldableItemSO.Name != "Knife") return; //for quick test TODO: fix this later
        choppingHandler.RequestToCutServerRpc();
    }


}