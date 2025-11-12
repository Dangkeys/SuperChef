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
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen

        if (Physics.Raycast(ray, out var hit, maxPickupDistance))
        {
            if (hit.collider.TryGetComponent(out InventoryItem inventoryItem))
            {
                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    InventorySlot currentInventorySlot = InventorySlots[i];
                    if (currentInventorySlot.InventoryItemSO == inventoryItem.InventoryItemSO)
                    {
                        int maximumAmount = inventoryItem.InventoryItemSO.MaximumAmount;

                        if (currentInventorySlot.CurrentAmount >= maximumAmount) continue;
                        currentInventorySlot.IncrementCurrentAmount();
                        Destroy(inventoryItem.gameObject);
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
                        Destroy(inventoryItem.gameObject);
                        return;
                    }

                }

            }
        }
    }
    private void PopInventoryAndSpawn(int index)
    {
        
    }
}
