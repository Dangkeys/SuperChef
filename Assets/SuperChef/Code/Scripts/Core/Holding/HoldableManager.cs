using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;




public class HoldableManager : NetworkBehaviour
{
    private InventoryItemSO currentInventoryItemSO;
    private Inventory inventory;
    private PickUp pickUp;
    private GameObject visualizeHoldableObject;

    [SerializeField] private HoldableProvider holdableProvider;
    [Inject]
    private void Init(Inventory inventory, PickUp pickUp)
    {
        this.inventory = inventory;
        this.pickUp = pickUp;
        currentInventoryItemSO = inventory.InventorySlots[0].InventoryItemSO;
        inventory.OnSelectedSlotIndexChanged += StartVisualizeHoldable;
        inventory.OnInventoryChanged += StartVisualizeHoldable;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        pickUp.OnCurrentPickableObjectChanged += StartVisualizeHoldable;
    }
    public override void OnNetworkDespawn()
    {
        inventory.OnSelectedSlotIndexChanged -= StartVisualizeHoldable;
        inventory.OnInventoryChanged -= StartVisualizeHoldable;
        if (!IsOwner) return;
        pickUp.OnCurrentPickableObjectChanged -= StartVisualizeHoldable;

    }

    private void StartVisualizeHoldable()
    {
        currentInventoryItemSO = inventory.InventorySlots[inventory.SelectedInventorySlotIndex].InventoryItemSO;
        holdableProvider.RequestToSetActiveHoldableServerRpc(pickUp.CurrentPickableObject == null ? (currentInventoryItemSO?.ID ?? "") : "");
    }

}