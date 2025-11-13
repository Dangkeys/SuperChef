using System;

[Serializable]
public class InventorySlot
{
    public event Action OnSlotChanged;
    
    public InventoryItemSO InventoryItemSO { get; private set; }
    public int CurrentAmount { get; private set; }

    public void SetInventoryItemSO(InventoryItemSO inventoryItemSO)
    {
        InventoryItemSO = inventoryItemSO;
        OnSlotChanged?.Invoke();
    }

    public void SetCurrentAmount(int amount)
    {
        CurrentAmount = amount;
        OnSlotChanged?.Invoke();
    }

    public void IncrementCurrentAmount()
    {
        CurrentAmount++;
        OnSlotChanged?.Invoke();
    }

    public void DecrementCurrentAmount()
    {
        CurrentAmount--;
        if (CurrentAmount <= 0)
        {
            Reset();
        }
        else
        {
            OnSlotChanged?.Invoke();
        }
    }

    public void Reset()
    {
        InventoryItemSO = null;
        CurrentAmount = 0;
        OnSlotChanged?.Invoke();
    }

    public bool IsEmpty() => InventoryItemSO == null || CurrentAmount <= 0;
}