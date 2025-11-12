using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [field: SerializeField] public InventoryItemSO InventoryItemSO { get; private set; }
    [field: SerializeField] public int CurrentAmount { get; private set; }
    [field: SerializeField] public InventorySlotUI InventorySlotUI{get; private set;}

    public void SetInventoryItemSO(InventoryItemSO inventoryItemSO)
    {
        InventoryItemSO = inventoryItemSO;
        InventorySlotUI.UpdateUI(this);
    }

    public int IncrementCurrentAmount(int amount = 1)
    {
        if (InventoryItemSO == null || amount < 0) return 0;

        CurrentAmount = Mathf.Clamp(CurrentAmount + amount, 0, InventoryItemSO.MaximumAmount);
        InventorySlotUI.UpdateUI(this);
        return InventoryItemSO.MaximumAmount - CurrentAmount;
    }
    public void DecrementCurrentAmount(int amount = 1)
    {
        if (InventoryItemSO == null || amount < 0) return ;


        CurrentAmount = Mathf.Clamp(CurrentAmount - amount, 0, InventoryItemSO.MaximumAmount);
        InventorySlotUI.UpdateUI(this);
    }
    public void SetCurrentAmount(int amount)
    {
        CurrentAmount = Mathf.Clamp(amount, 0, InventoryItemSO.MaximumAmount);
        InventorySlotUI.UpdateUI(this);
    }
    public void Reset()
    {
        InventoryItemSO = null;
        CurrentAmount = 0;
        InventorySlotUI.UpdateUI(this);
    }

}
