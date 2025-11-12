using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [field: SerializeField] public InventoryItemSO InventoryItemSO { get; private set; }
    [field: SerializeField] public int CurrentAmount { get; private set; }

    public void SetInventoryItemSO(InventoryItemSO inventoryItemSO)
    {
        InventoryItemSO = inventoryItemSO;
    }

    public int IncrementCurrentAmount(int amount = 1)
    {
        if (InventoryItemSO == null || amount < 0) return 0;

        CurrentAmount = Mathf.Clamp(CurrentAmount + amount, 0, InventoryItemSO.MaximumAmount);
        return InventoryItemSO.MaximumAmount - CurrentAmount;
    }
    public void DecrementCurrentAmount(int amount = 1)
    {
        if (InventoryItemSO == null || amount < 0) return ;


        CurrentAmount = Mathf.Clamp(CurrentAmount - amount, 0, InventoryItemSO.MaximumAmount);
    }
    public void SetCurrentAmount(int amount)
    {
        CurrentAmount = Mathf.Clamp(amount, 0, InventoryItemSO.MaximumAmount);
    }

}
