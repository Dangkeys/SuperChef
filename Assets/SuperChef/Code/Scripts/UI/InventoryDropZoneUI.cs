using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InventoryDropZoneUI : MonoBehaviour, IDropHandler
{
    private Inventory inventory;
    private SignalBus signalBus;
    [Inject]
    public void Construct(SignalBus signalBus, GameInputReader gameInputReader)
    {
        this.signalBus = signalBus;
        signalBus.Subscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
        
    }
    private void OnPlayerSpawned(PlayerSpawnedSignal signal)
    {
        Init(signal.Inventory);
    }
     public void Init(Inventory playerInventory)
    {
        inventory = playerInventory;
    }

    public void OnDrop(PointerEventData eventData)
    {

        var draggedSlotUI = eventData.pointerDrag?.GetComponentInParent<InventorySlotUI>();
        if (draggedSlotUI == null) return;

        var draggedSlot = draggedSlotUI.InventorySlot;
        if (draggedSlot == null || draggedSlot.IsEmpty()) return;

        if (inventory != null)
        {
            inventory.DropItem(draggedSlot);
        }
    }
}
