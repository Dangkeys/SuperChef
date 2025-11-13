using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Unity.Netcode;
using log4net.Appender;

public class Inventory : NetworkBehaviour
{
    private const int MAX_SLOT_COUNT = 20;
    public int MaxHotBarSlotAmount { get; private set; } = 4;

    public InventorySlot[] InventorySlots { get; private set; } = new InventorySlot[MAX_SLOT_COUNT];

    [SerializeField]
    private float maxPickupDistance = 10f;

    private GameInputReader inputReader;
    private InventoryItemProvider inventoryItemProvider;
    public InventorySlot SelectedInventorySlot { get; private set; }
    private PickUp pickUp;

    private void Awake()
    {
        for (int i = 0; i < MAX_SLOT_COUNT; i++)
        {
            InventorySlots[i] = new InventorySlot();
        }
    }

    [Inject]
    private void Init(GameInputReader inputReader, InventoryItemProvider inventoryItemProvider, PickUp pickUp)
    {
        this.inputReader = inputReader;
        this.inventoryItemProvider = inventoryItemProvider;
        this.pickUp = pickUp;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent += HandleInteract;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent -= HandleInteract;
    }

    private void HandleInteract()
    {
        if (!Camera.main) return;

        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (!Physics.Raycast(ray, out var hit, maxPickupDistance)) return;
        if (!hit.collider.TryGetComponent(out InventoryItem inventoryItem)) return;
        if (!inventoryItem.TryGetComponent(out NetworkObject networkObject)) return;

        var networkRef = new NetworkObjectReference(networkObject);
        TryAddItemToInventory(inventoryItem, networkRef);
    }

    private void TryAddItemToInventory(InventoryItem inventoryItem, NetworkObjectReference networkRef)
    {
        var itemSO = inventoryItem.InventoryItemSO;
        if (itemSO == null) return;

        // 1️⃣ Try stacking with existing slot
        foreach (var slot in InventorySlots)
        {
            if (slot.InventoryItemSO != itemSO) continue;
            if (slot.CurrentAmount >= itemSO.MaximumAmount) continue;

            slot.IncrementCurrentAmount();
            DespawnItemServerRpc(networkRef);
            return;
        }

        // 2️⃣ Try placing in empty slot
        foreach (var slot in InventorySlots)
        {
            if (slot.InventoryItemSO != null) continue;

            slot.SetInventoryItemSO(itemSO);
            slot.SetCurrentAmount(1);
            DespawnItemServerRpc(networkRef);
            return;
        }
    }

    public void SwapInventorySlot(InventorySlot a, InventorySlot b)
    {
        if (a == null || b == null || a == b) return;

        var tempSO = a.InventoryItemSO;
        var tempAmount = a.CurrentAmount;

        a.SetInventoryItemSO(b.InventoryItemSO);
        a.SetCurrentAmount(b.CurrentAmount);

        b.SetInventoryItemSO(tempSO);
        b.SetCurrentAmount(tempAmount);
    }


    private void RemoveInventorySlotAndSpawn(InventorySlot slot)
    {
        if (slot.InventoryItemSO == null) return;

        Vector3 spawnPos = transform.position + transform.forward * 3f;
        SpawnDroppedItemServerRpc(slot.InventoryItemSO.name, spawnPos);
        slot.Reset();
    }


    [ServerRpc]
    private void DespawnItemServerRpc(NetworkObjectReference objRef)
    {
        if (!objRef.TryGet(out NetworkObject netObj)) return;
        if (netObj != null && netObj.IsSpawned)
        {
            netObj.Despawn(true);
        }
    }

    [ServerRpc]
    private void SpawnDroppedItemServerRpc(string itemSOName, Vector3 spawnPosition)
    {
        var itemSO = inventoryItemProvider.GetInventoryItemSOByName(itemSOName);
        if (itemSO == null) return;

        var prefab = inventoryItemProvider.GetInventoryItemBySO(itemSO);
        var spawnedItem = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (spawnedItem.TryGetComponent(out NetworkObject netObj))
        {
            netObj.Spawn();
        }
    }
}
