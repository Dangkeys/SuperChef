using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public Stack<InventoryItemSO> inventoryItemSOs { get; private set; } = new Stack<InventoryItemSO>();
    private GameInputReader _inputReader;


    [SerializeField] private float maxPickupDistance = 10f;

    [Inject]
    private void Init(GameInputReader inputReader)
    {
        _inputReader = inputReader;

    }
    private void Start()
    {
        _inputReader.InteractEvent += OnTryAddToInventory;
        _inputReader.AttackEvent += TryPopStack;
    }
    private void OnDestroy()
    {
        _inputReader.InteractEvent -= OnTryAddToInventory;
        _inputReader.AttackEvent -= TryPopStack;
    }



    private void OnTryAddToInventory()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen

        if (Physics.Raycast(ray, out var hit, maxPickupDistance))
        {
            if (hit.collider.TryGetComponent(out InventoryItem inventoryItem))
            {
                inventoryItemSOs.Push(inventoryItem.inventoryItemSO);
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
            Instantiate(inventoryItemSO.InventoryItemPrefab, transform.position + transform.forward * 2, Quaternion.identity);
        }
    }
}
