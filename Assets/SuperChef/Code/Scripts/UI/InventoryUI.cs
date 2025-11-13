using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private Transform inventorySlotsParentTransform;
    [SerializeField] private Transform hotBarSlotsParentTransform;
    [AssetsOnly]
    [SerializeField] private InventorySlotUI inventorySlotUIPrefab;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        signalBus.Subscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(PlayerSpawnedSignal signal)
    {
        Init(signal.Inventory);
    }

    public void Init(Inventory playerInventory)
    {
        inventory = playerInventory;
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        // Clear existing slots
        foreach (Transform child in inventorySlotsParentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in hotBarSlotsParentTransform)
        {
            Destroy(child.gameObject);
        }

        // Create new slots
        for (int i = 0; i < inventory.InventorySlots.Length; i++)
        {
            var inventorySlot = inventory.InventorySlots[i];
            var parent = i < inventory.MaxHotBarSlotAmount ? hotBarSlotsParentTransform : inventorySlotsParentTransform;

            var inventorySlotUI = Instantiate(inventorySlotUIPrefab, parent);
            inventorySlotUI.Init(inventorySlot);
        }
    }
}