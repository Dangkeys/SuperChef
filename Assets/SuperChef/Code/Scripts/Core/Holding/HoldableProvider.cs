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

    public InventoryItemSO CurrentHoldableItemSO { get; private set; }

    private Dictionary<InventoryItemSO, GameObject> soToHoldableObjectDict;

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {

        if (holdableItemEntries == null) return;

        if (soToHoldableObjectDict != null) return;

        soToHoldableObjectDict = new Dictionary<InventoryItemSO, GameObject>();

        foreach (var entry in holdableItemEntries)
        {
            if (entry.HoldableItemSO == null || entry.HoldableObjectGO == null)
            {
                Debug.LogWarning("HoldableItemEntry has missing HoldableItemSO or HoldableObjectGO reference!", this);
                continue;
            }
            if (!soToHoldableObjectDict.ContainsKey(entry.HoldableItemSO))
            {
                soToHoldableObjectDict.Add(entry.HoldableItemSO, entry.HoldableObjectGO);
            }
        }
    }
    public GameObject GetInventoryItemBySO(InventoryItemSO so)
    {
        if (soToHoldableObjectDict == null) InitializeDictionary();

        if (so == null) return null;
        return soToHoldableObjectDict.TryGetValue(so, out var item) ? item : null;
    }

    public InventoryItemSO GetInventoryItemSOByID(string id)
    {
        if (soToHoldableObjectDict == null) InitializeDictionary();

        foreach (InventoryItemSO item in soToHoldableObjectDict.Keys)
        {
            if (item.ID == id)
                return item;
        }
        return null;
    }
    public void VisualizeHoldableItem(string holdableId)
    {
        CurrentHoldableItemSO = null;
        
        foreach (KeyValuePair<InventoryItemSO, GameObject> entry in soToHoldableObjectDict)
        {
            bool isEqual = entry.Key.ID == holdableId;
            entry.Value.SetActive(isEqual);

            if (isEqual)
            {
            CurrentHoldableItemSO = entry.Key;
            }
        }
    }


    [Rpc(SendTo.Server)]
    public void RequestToSetActiveHoldableServerRpc(string holdableID)
    {
        NotifyToSetHoldableActiveClientRpc(holdableID);
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyToSetHoldableActiveClientRpc(string holdableID)
    {
        VisualizeHoldableItem(holdableID);
    }
}