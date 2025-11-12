using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Unity.Netcode;
public class Inventory : NetworkBehaviour
{

    private const int MAXIMUM_SLOT_AMOUNT = 40;
    [field: SerializeField] public InventorySlot[] InventorySlots { get; private set; } = new InventorySlot[MAXIMUM_SLOT_AMOUNT];
    private GameInputReader inputReader;
    private InventoryItemProvider inventoryItemProvider;


    [SerializeField] private float maxPickupDistance = 10f;
    [Inject]
    private void Init(GameInputReader inputReader, InventoryItemProvider inventoryItemProvider)
    {
        this.inputReader = inputReader;
        this.inventoryItemProvider = inventoryItemProvider;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent += OnTryAddToInventory;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent -= OnTryAddToInventory;
    }

    private void OnTryAddToInventory()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out var hit, maxPickupDistance))
        {
            if (hit.collider.TryGetComponent(out InventoryItem inventoryItem))
            {
                if (!inventoryItem.TryGetComponent(out NetworkObject networkObject)) return;
                NetworkObjectReference networkObjectReference = new NetworkObjectReference(networkObject);
                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    InventorySlot currentInventorySlot = InventorySlots[i];
                    if (currentInventorySlot.InventoryItemSO == inventoryItem.InventoryItemSO)
                    {
                        int maximumAmount = inventoryItem.InventoryItemSO.MaximumAmount;

                        if (currentInventorySlot.CurrentAmount >= maximumAmount) continue;
                        currentInventorySlot.IncrementCurrentAmount();
                        DespawnInventoryItemServerRpc(networkObjectReference);

                        return;
                    }

                }

                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    InventorySlot currentInventorySlot = InventorySlots[i];
                    if (currentInventorySlot.InventoryItemSO != null) continue;
                    {
                        currentInventorySlot.SetInventoryItemSO(inventoryItem.InventoryItemSO);
                        currentInventorySlot.SetCurrentAmount(1);

                        DespawnInventoryItemServerRpc(networkObjectReference);

                        return;
                    }

                }

            }
        }
    }

    public void SwapInventorySlot(InventorySlot inventorySlotA, InventorySlot inventorySlotB)
    {
        if (inventorySlotA == null || inventorySlotB == null || inventorySlotA == inventorySlotB) return;

        var itemA = inventorySlotA.InventoryItemSO;
        var amountA = inventorySlotA.CurrentAmount;

        inventorySlotA.SetInventoryItemSO(inventorySlotB.InventoryItemSO);
        inventorySlotA.SetCurrentAmount(inventorySlotB.CurrentAmount);

        inventorySlotB.SetInventoryItemSO(itemA);
        inventorySlotB.SetCurrentAmount(amountA);
    }


    private void RemoveInventorySlotAndSpawn(InventorySlot inventorySlot)
    {
        if (inventorySlot.InventoryItemSO == null) return;

        float offsetAmount = 3f;
        Vector3 spawnPosition = transform.position + Vector3.forward * offsetAmount;

        SpawnDroppedItemServerRpc(inventorySlot.InventoryItemSO.name, spawnPosition);

        inventorySlot.Reset();
    }
    [ServerRpc]
    private void DespawnInventoryItemServerRpc(NetworkObjectReference networkObjectReference)
    {
        if (!networkObjectReference.TryGet(out NetworkObject networkObject)) return;
        if (networkObject.IsSpawned)
        {
            networkObject.Despawn(true);
        }
    }
    [ServerRpc]
    private void SpawnDroppedItemServerRpc(string itemSOName, Vector3 spawnPosition)
    {
        InventoryItemSO itemSO = inventoryItemProvider.GetInventoryItemSOByName(itemSOName);
        if (itemSO == null) return;

        InventoryItem inventoryItem = Instantiate(inventoryItemProvider.GetInventoryItemBySO(itemSO), spawnPosition, Quaternion.identity);

        if (inventoryItem.TryGetComponent(out NetworkObject networkObject))
        {

            networkObject.Spawn();
        }
    }


}
