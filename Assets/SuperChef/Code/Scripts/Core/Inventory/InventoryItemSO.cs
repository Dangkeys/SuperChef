using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Scriptable Objects/InventoryItemSO")]
public class InventoryItemSO : BaseSO
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }

    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [Range(1,40)]
    [field: SerializeField] public int MaximumAmount { get; private set; } = 1;
    public bool IsStackable => MaximumAmount > 1;
}
