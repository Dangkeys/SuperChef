using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

[System.Serializable]
public class HoldableItemEntry
{
    [field: SerializeField] public InventoryItemSO HoldableItemSO { get; private set; }
    [field: SerializeField] public GameObject HoldableObjectGO { get; private set; }
}

public class HoldableProvider : NetworkBehaviour
{
    [SerializeField] private List<HoldableItemEntry> holdableItemEntries = new();

    [field: SerializeField] public InventoryItemSO CurrentHoldableItemSO { get; private set; }

    private Dictionary<InventoryItemSO, GameObject> soToHoldableObjectDict;

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        // Safety check: ensure the list isn't null
        if (holdableItemEntries == null) return;

        if (soToHoldableObjectDict != null) return;

        soToHoldableObjectDict = new Dictionary<InventoryItemSO, GameObject>();

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
    public GameObject GetInventoryItemBySO(InventoryItemSO so)
    {
        // Safety: Ensure dict is initialized if Get is called before OnEnable (rare but possible)
        if (soToHoldableObjectDict == null) InitializeDictionary();

        if (so == null) return null;
        return soToHoldableObjectDict.TryGetValue(so, out var item) ? item : null;
    }

    public InventoryItemSO GetInventoryItemSOByID(string id)
    {
        if (soToHoldableObjectDict == null) InitializeDictionary();

        // Optimization note: Looping through keys is O(n). 
        // If this list is huge, consider a second dictionary for name lookups.
        foreach (InventoryItemSO item in soToHoldableObjectDict.Keys)
        {
            // Assuming InventoryItemSO has a "Name" property. 
            // If it's the asset name, use item.name
            if (item.ID == id)
                return item;
        }
        return null;
    }
    public void VisualizeHoldableItem(string holdableId)
    {
        foreach (var entry in soToHoldableObjectDict)
        {
            InventoryItemSO item = entry.Key;
            GameObject holdableObject = entry.Value;
            var isEqual = item.ID == holdableId;
            holdableObject.SetActive(isEqual);

            if(isEqual)
            {
                CurrentHoldableItemSO = GetInventoryItemSOByID(holdableId);
            }
        }
    }

    [ServerRpc]
    public void RequestToSetActiveHoldableServerRpc(string holdableID)
    {
        NotifyToSetActiveHoldableClientRpc(holdableID);
    }
    [ClientRpc]
    private void NotifyToSetActiveHoldableClientRpc(string holdableID)
    {
        VisualizeHoldableItem(holdableID);
    }
}