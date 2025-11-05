using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Scriptable Objects/InventoryItemSO")]
public class InventoryItemSO : ScriptableObject
{
    [field: SerializeField] public String Name { get; private set; }
    [field: SerializeField] public String Description { get; private set; }

    [AssetsOnly]
    [field: SerializeField] public GameObject InventoryItemPrefab { get; private set; }
    [field: SerializeField] public Image ItemUI { get; private set; }
}
