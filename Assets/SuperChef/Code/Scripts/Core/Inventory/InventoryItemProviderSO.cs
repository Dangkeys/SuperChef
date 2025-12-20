using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class InventoryItemEntry
{

    [field: SerializeField] public InventoryItemSO ItemSO { get; private set; }
    [field: SerializeField] public InventoryItem Item { get; private set; }

    //for inspector visuals
    [ShowInInspector, HideLabel]
    private Sprite PreviewSO => ItemSO?.ItemSprite;

    [ShowInInspector, HideLabel]
    private GameObject PreviewItem => Item?.gameObject;
}

[CreateAssetMenu(fileName = "InventoryItemProvider", menuName = "Scriptable Objects/ItemProviderSO")]
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

        if (inventoryItemEntries == null) return;

        if (soToInventoryItemDict != null) return;

        soToInventoryItemDict = new Dictionary<InventoryItemSO, InventoryItem>();

        foreach (var entry in inventoryItemEntries)
        {
            if (entry.ItemSO == null || entry.Item == null)
            {

                Debug.LogWarning("InventoryItemEntry has missing ItemSO or Item reference!", this);
                continue;
            }

            if (!soToInventoryItemDict.ContainsKey(entry.ItemSO))
            {
                soToInventoryItemDict.Add(entry.ItemSO, entry.Item);
            }
        }
    }

    public InventoryItem GetInventoryItemBySO(InventoryItemSO so)
    {

        if (soToInventoryItemDict == null) InitializeDictionary();

        if (so == null) return null;
        return soToInventoryItemDict.TryGetValue(so, out var item) ? item : null;
    }

    public InventoryItemSO GetInventoryItemSOByID(string id)
    {
        if (soToInventoryItemDict == null) InitializeDictionary();


        foreach (InventoryItemSO item in soToInventoryItemDict.Keys)
        {

            if (item.ID == id)
                return item;
        }
        return null;
    }
    public InventoryItem GetInventoryItemByID(string id)
    {
        var so = GetInventoryItemSOByID(id);
        return GetInventoryItemBySO(so);
    }
}