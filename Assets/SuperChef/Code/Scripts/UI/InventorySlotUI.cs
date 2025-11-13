using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [field: SerializeField] public InventoryItemUI InventoryItemUI;
    [field: SerializeField] public Image BackgroundImage { get; private set; }
    public void Init(InventorySlot inventorySlot)
    {
        InventoryItemUI.Init(inventorySlot);
    }
}