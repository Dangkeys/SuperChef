using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform inventorySlotsParentTransform;
    [SerializeField] private Transform hotBarSlotsParentTransform;
    [SerializeField] private GameObject showInventoryGameObject;
    [AssetsOnly]
    [SerializeField] private InventorySlotUI inventorySlotUIPrefab;
    private Inventory inventory;
    private GameInputReader inputReader;
    public bool IsOpenInventory { get; private set; } = false;
    private SignalBus signalBus;
    private UIOpenSignal uIOpenSignal = new UIOpenSignal(UIType.Inventory);
    [Inject]
    public void Construct(SignalBus signalBus, GameInputReader gameInputReader)
    {
        this.signalBus = signalBus;
        signalBus.Subscribe<PlayerSpawnedSignal>(OnPlayerSpawned);

        inputReader = gameInputReader;
        inputReader.OpenInventoryEvent += ToggleShowUI;
        showInventoryGameObject.SetActive(IsOpenInventory);
        
    }
    private void OnPlayerSpawned(PlayerSpawnedSignal signal)
    {
        Init(signal.Inventory);
    }
     public void Init(Inventory playerInventory)
    {
        inventory = playerInventory;
        

        SetupInventorySlots();
    }

    private void ToggleShowUI()
    {
        IsOpenInventory = !IsOpenInventory;
        showInventoryGameObject.SetActive(IsOpenInventory);

        uIOpenSignal.IsOpen = IsOpenInventory;
        signalBus.Fire(uIOpenSignal);

    }

    void OnDestroy()
    {
        if(signalBus != null)
        {
            signalBus.TryUnsubscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
        }

        if (inputReader == null) return;
        inputReader.OpenInventoryEvent -= ToggleShowUI;
    }



   

    private void SetupInventorySlots()
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
            var parent = i < Inventory.MAX_HOTBAR_SLOT_COUNT ? hotBarSlotsParentTransform : inventorySlotsParentTransform;

            var inventorySlotUI = Instantiate(inventorySlotUIPrefab, parent);
            inventorySlotUI.Initialize(inventorySlot, inventory);
        }
    }
}