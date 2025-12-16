using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;




public class HoldableManager : NetworkBehaviour
{
    private HoldableItemSO currentHoldableItemSO;
    private Inventory inventory;
    private GameObject visualizeHoldableObject;

    [SerializeField] private HoldableProvider holdableProvider;
    [Inject]
    private void Init(Inventory inventory)
    {
        this.inventory = inventory;
        currentHoldableItemSO = inventory.InventorySlots[0].InventoryItemSO as HoldableItemSO;
        inventory.OnSelectedSlotIndexChanged += ChangeCurrentHoldable;
        inventory.OnInventoryChanged += InventoryChanged;
    }

    private void ChangeCurrentHoldable()
    {
        StartVisualizeHoldable();
    }

    private void InventoryChanged()
    {
        StartVisualizeHoldable();
    }

    private void StartVisualizeHoldable()
    {
        currentHoldableItemSO = inventory.InventorySlots[inventory.SelectedInventorySlotIndex].InventoryItemSO  as HoldableItemSO;
        holdableProvider.RequestToSetActiveHoldableServerRpc(currentHoldableItemSO?.ID ?? "");
    }

}