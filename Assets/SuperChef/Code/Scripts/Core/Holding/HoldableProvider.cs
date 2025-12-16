using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

[System.Serializable]
public class HoldableItemEntry
{
    [field: SerializeField] public HoldableItemSO HoldableItemSO { get; private set; }
    [field: SerializeField] public GameObject HoldableObjectGO { get; private set; }
}

public class HoldableProvider : NetworkBehaviour
{
    [SerializeField] private List<HoldableItemEntry> holdableItemEntries = new();


    private Dictionary<HoldableItemSO, GameObject> soToHoldableObjectDict;

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        // Safety check: ensure the list isn't null
        if (holdableItemEntries == null) return;

        if (soToHoldableObjectDict != null) return;

        soToHoldableObjectDict = new Dictionary<HoldableItemSO, GameObject>();

        foreach (var entry in holdableItemEntries)
        {
            if (entry.HoldableItemSO == null || entry.HoldableObjectGO == null)
            {
                // Changed 'this' context slightly; Debug works fine, but clicking it 
                // highlights the SO asset in the project window now.
                Debug.LogWarning("HoldableItemEntry has missing HoldableItemSO or HoldableObjectGO reference!", this);
                continue;
            }
            // Use TryAdd or check key existence to prevent duplicate key errors
            if (!soToHoldableObjectDict.ContainsKey(entry.HoldableItemSO))
            {
                soToHoldableObjectDict.Add(entry.HoldableItemSO, entry.HoldableObjectGO);
            }
        }
    }
    public GameObject GetInventoryItemBySO(HoldableItemSO so)
    {
        // Safety: Ensure dict is initialized if Get is called before OnEnable (rare but possible)
        if (soToHoldableObjectDict == null) InitializeDictionary();

        if (so == null) return null;
        return soToHoldableObjectDict.TryGetValue(so, out var item) ? item : null;
    }

    public HoldableItemSO GetInventoryItemSOByName(string name)
    {
        if (soToHoldableObjectDict == null) InitializeDictionary();

        // Optimization note: Looping through keys is O(n). 
        // If this list is huge, consider a second dictionary for name lookups.
        foreach (HoldableItemSO item in soToHoldableObjectDict.Keys)
        {
            // Assuming InventoryItemSO has a "Name" property. 
            // If it's the asset name, use item.name
            if (item.Name == name)
                return item;
        }
        return null;
    }
    public void VisualizeHoldableItem(string holdableItemName)
    {
        foreach (var entry in soToHoldableObjectDict)
        {
            HoldableItemSO item = entry.Key;
            GameObject holdableObject = entry.Value;
            holdableObject.SetActive(item.Name == holdableItemName);
        }
    }

    [ServerRpc]
    public void RequestToSetActiveHoldableServerRpc(string holdableItemName)
    {
        NotifyToSetActiveHoldableClientRpc(holdableItemName);
    }
    [ClientRpc]
    private void NotifyToSetActiveHoldableClientRpc(string holdableItemName)
    {
        VisualizeHoldableItem(holdableItemName);
    }
}