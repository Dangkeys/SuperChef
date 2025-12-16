using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class InventoryItemEntry
{
    [field: SerializeField] public InventoryItemSO ItemSO { get; private set; }
    [field: SerializeField] public InventoryItem Item { get; private set; }
}

[CreateAssetMenu(fileName = "InventoryItemProvider", menuName = "Scriptable Objects/Item Provider")]
public class InventoryItemProviderSO : ScriptableObject
{
    [SerializeField] private List<InventoryItemEntry> inventoryItemEntries = new();


    private Dictionary<InventoryItemSO, InventoryItem> soToInventoryItemDict;

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        // Safety check: ensure the list isn't null
        if (inventoryItemEntries == null) return;

        if (soToInventoryItemDict != null) return;
        
        soToInventoryItemDict = new Dictionary<InventoryItemSO, InventoryItem>();
        
        foreach (var entry in inventoryItemEntries)
        {
            if (entry.ItemSO == null || entry.Item == null)
            {
                // Changed 'this' context slightly; Debug works fine, but clicking it 
                // highlights the SO asset in the project window now.
                Debug.LogWarning("InventoryItemEntry has missing ItemSO or Item reference!", this);
                continue;
            }
            // Use TryAdd or check key existence to prevent duplicate key errors
            if (!soToInventoryItemDict.ContainsKey(entry.ItemSO))
            {
                soToInventoryItemDict.Add(entry.ItemSO, entry.Item);
            }
        }
    }

    public InventoryItem GetInventoryItemBySO(InventoryItemSO so)
    {
        // Safety: Ensure dict is initialized if Get is called before OnEnable (rare but possible)
        if (soToInventoryItemDict == null) InitializeDictionary(); 

        if (so == null) return null;
        return soToInventoryItemDict.TryGetValue(so, out var item) ? item : null;
    }

    public InventoryItemSO GetInventoryItemSOByName(string name)
    {
        if (soToInventoryItemDict == null) InitializeDictionary();

        // Optimization note: Looping through keys is O(n). 
        // If this list is huge, consider a second dictionary for name lookups.
        foreach (InventoryItemSO item in soToInventoryItemDict.Keys)
        {
            // Assuming InventoryItemSO has a "Name" property. 
            // If it's the asset name, use item.name
            if (item.Name == name) 
                return item;
        }
        return null;
    }
}