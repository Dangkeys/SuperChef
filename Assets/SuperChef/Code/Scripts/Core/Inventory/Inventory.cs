using UnityEngine;
using Zenject;
using Unity.Netcode;

public class Inventory : NetworkBehaviour
{
    private const int MAX_SLOT_COUNT = 20;
    public const int MAX_HOTBAR_SLOT_COUNT = 4;

    public InventorySlot[] InventorySlots { get; private set; } = new InventorySlot[MAX_SLOT_COUNT];

    [SerializeField]
    private float maxPickupDistance = 10f;

    private GameInputReader inputReader;
    private InventoryItemProvider inventoryItemProvider;
    [Range(0, MAX_HOTBAR_SLOT_COUNT - 1)]
    public int SelectedInventorySlotIndex { get; private set; }
    private PickUp pickUp;
    public System.Action OnSelectedSlotIndexChanged;
    public System.Action OnInventoryChanged;

    private void Awake()
    {
        for (int i = 0; i < MAX_SLOT_COUNT; i++)
        {
            InventorySlots[i] = new InventorySlot();
        }
        SelectedInventorySlotIndex = 0;
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
        inputReader.NextEvent += NextSelectedSlot;
        inputReader.PreviousEvent += PreviousSelectedSlot;
        inputReader.SlotChangedEvent += ChangeSelectedSlot;
    }

    private void ChangeSelectedSlot(int slotIndex)
    {
        SelectedInventorySlotIndex = slotIndex;
        OnSelectedSlotIndexChanged?.Invoke();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent -= HandleInteract;
        inputReader.NextEvent -= NextSelectedSlot;
        inputReader.PreviousEvent -= PreviousSelectedSlot;
        inputReader.SlotChangedEvent -= ChangeSelectedSlot;
    }
    private void NextSelectedSlot()
    {
        SelectedInventorySlotIndex = (SelectedInventorySlotIndex + 1) % MAX_HOTBAR_SLOT_COUNT;
        OnSelectedSlotIndexChanged?.Invoke();
    }

    private void PreviousSelectedSlot()
    {
        SelectedInventorySlotIndex--;
        if (SelectedInventorySlotIndex < 0)
            SelectedInventorySlotIndex = MAX_HOTBAR_SLOT_COUNT - 1;
        OnSelectedSlotIndexChanged?.Invoke();
    }

    private void HandleInteract()
    {
        if (!Camera.main) return;

        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (!Physics.Raycast(ray, out var hit, maxPickupDistance)) return;
        if (!hit.collider.TryGetComponent(out InventoryItem inventoryItem)) return;
        if (!inventoryItem.TryGetComponent(out NetworkObject networkObject)) return;
        if (pickUp.CurrentPickableObject != null) return;
        
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
            OnInventoryChanged?.Invoke();
            return;
        }

        // 2️⃣ Try placing in empty slot
        foreach (var slot in InventorySlots)
        {
            if (slot.InventoryItemSO != null) continue;

            slot.SetInventoryItemSO(itemSO);
            slot.SetCurrentAmount(1);
            DespawnItemServerRpc(networkRef);
            OnInventoryChanged?.Invoke();
            return;
        }
    }
    public int GetSlotIndex(InventorySlot slot)
    {
        int index = System.Array.IndexOf(InventorySlots, slot);
        if (index == -1)
        {
            throw new System.ArgumentException($"Slot not found in inventory", nameof(slot));
        }
        return index;
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
        OnInventoryChanged?.Invoke();
    }


    public void DropItem(InventorySlot slot)
    {
        if (slot.InventoryItemSO == null) return;

        Vector3 randomOffset = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0f, 0.5f),
            Random.Range(-1f, 1f)
        );
        Vector3 spawnPos = transform.position + transform.forward * 2f + randomOffset;
        SpawnDroppedItemServerRpc(slot.InventoryItemSO.Name, spawnPos);
        slot.DecrementCurrentAmount();
        OnInventoryChanged?.Invoke();
    }

    public void DecrementSelectedSlot()
    {
        InventorySlots[SelectedInventorySlotIndex].DecrementCurrentAmount();
        OnInventoryChanged?.Invoke();
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
