using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class InventoryItemEntry
{
    [field: SerializeField] public InventoryItemSO ItemSO { get; private set; }
    [field: SerializeField] public InventoryItem Item { get; private set; }
}

public class InventoryItemProvider : MonoBehaviour
{
    [SerializeField] private List<InventoryItemEntry> inventoryItemEntries = new();

    private Dictionary<InventoryItemSO, InventoryItem> soToInventoryItemDict;

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        if (soToInventoryItemDict != null) return;
        
        soToInventoryItemDict = new Dictionary<InventoryItemSO, InventoryItem>();
        
        foreach (var entry in inventoryItemEntries)
        {
            if (entry.ItemSO == null || entry.Item == null)
            {
                Debug.LogWarning("InventoryItemEntry has missing ItemSO or Item reference!", this);
                continue;
            }
            soToInventoryItemDict[entry.ItemSO] = entry.Item;
        }
        
        if (soToInventoryItemDict.Count == 0)
        {
            Debug.LogError("No valid inventory items loaded!", this);
        }
    }


    public InventoryItem GetInventoryItemBySO(InventoryItemSO so)
    {
        if (so == null) return null;
        return soToInventoryItemDict.TryGetValue(so, out var item) ? item : null;
    }
    public InventoryItemSO GetInventoryItemSOByName(string name)
    {
        foreach (InventoryItemSO item in soToInventoryItemDict.Keys)
        {
            if (item.Name == name)
                return item;
        }
        return null;
    }

}