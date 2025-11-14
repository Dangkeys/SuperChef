using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image inventoryIconImage;
    [SerializeField] private TextMeshProUGUI currentAmountText;

    private InventorySlot inventorySlot;

    public void Init(InventorySlot slot)
    {
        inventorySlot = slot;
        inventorySlot.OnSlotChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        if (inventorySlot != null)
        {
            inventorySlot.OnSlotChanged -= UpdateDisplay;
        }
    }

    private void UpdateDisplay()
    {
        if (inventorySlot.IsEmpty())
        {
            inventoryIconImage.sprite = null;
            inventoryIconImage.enabled = false;
            currentAmountText.text = "";
        }
        else
        {
            inventoryIconImage.sprite = inventorySlot.InventoryItemSO.ItemSprite;
            inventoryIconImage.enabled = true;
            
            if (inventorySlot.InventoryItemSO.IsStackable && inventorySlot.CurrentAmount > 1)
            {
                currentAmountText.text = inventorySlot.CurrentAmount.ToString();
            }
            else
            {
                currentAmountText.text = "";
            }
        }
    }

    public InventorySlot GetInventorySlot() => inventorySlot;
}