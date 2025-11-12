using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [field: SerializeField] public Image InventoryIconImage { get; private set; }
    [field: SerializeField] public TextMeshProUGUI CurrentAmountText { get; private set; }
    public void UpdateUI(InventorySlot inventorySlot)
    {
        if(inventorySlot.InventoryItemSO== null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
        InventoryIconImage = inventorySlot.InventoryItemSO.ItemIcon;
        CurrentAmountText.text = inventorySlot.CurrentAmount.ToString();
    }
}