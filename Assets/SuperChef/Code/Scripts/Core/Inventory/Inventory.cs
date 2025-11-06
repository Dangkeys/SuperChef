using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public Stack<InventoryItemSO> inventoryItemSOs { get; private set; } = new Stack<InventoryItemSO>();
    private GameInputReader inputReader;
    private InventoryItemProvider inventoryItemProvider;


    [SerializeField] private float maxPickupDistance = 10f;
    [Inject]
    private void Init(GameInputReader inputReader, InventoryItemProvider inventoryItemProvider)
    {
        this.inputReader = inputReader;
        this.inventoryItemProvider = inventoryItemProvider;
    }
    private void Start()
    {
        inputReader.InteractEvent += OnTryAddToInventory;
        inputReader.AttackEvent += TryPopStack;
    }
    private void OnDestroy()
    {
        inputReader.InteractEvent -= OnTryAddToInventory;
        inputReader.AttackEvent -= TryPopStack;
    }



    private void OnTryAddToInventory()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen

        if (Physics.Raycast(ray, out var hit, maxPickupDistance))
        {
            if (hit.collider.TryGetComponent(out InventoryItem inventoryItem))
            {
                inventoryItemSOs.Push(inventoryItem.InventoryItemSO);
                Debug.Log(inventoryItemSOs);
                Destroy(inventoryItem.gameObject);
            }
        }

    }
    private void TryPopStack()
    {
        if (inventoryItemSOs.Count > 0)
        {
            InventoryItemSO inventoryItemSO = inventoryItemSOs.Pop();
            Debug.Log("Drop " + inventoryItemSO.Name + "Its Detail is" + inventoryItemSO.Description);

            InventoryItem itemPrefab = inventoryItemProvider.GetInventoryItemBySO(inventoryItemSO);

            if (itemPrefab == null)
            {
                Debug.LogError($"InventoryItem prefab not found for SO: {inventoryItemSO.Name}", this);
                return;
            }

            float spawnOffest = 2.0f;
            Instantiate(itemPrefab, transform.position + transform.forward * spawnOffest, Quaternion.identity);
        }
    }
}
