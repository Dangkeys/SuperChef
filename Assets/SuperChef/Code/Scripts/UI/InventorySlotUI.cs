using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private InventoryItemUI inventoryItemUI;
    public InventorySlot InventorySlot { get; private set; }
    private Canvas canvas;
    private RectTransform draggedItemTransform;
    private CanvasGroup canvasGroup;
    private int slotIndex;
    private Transform originalParent;
    private static InventorySlotUI currentlyDraggedSlot; // Add this to track the dragged slot
    [SerializeField] private Sprite unselectedSlotSprite;

    [SerializeField] private Sprite selectedSlotSprite;
    private Inventory inventory;
    private Image backgroundImage;


    public void Initialize(InventorySlot slot, Inventory playerInventory)
    {
        inventory = playerInventory;
        InventorySlot = slot;
        slotIndex = inventory.GetSlotIndex(slot);
        inventory.OnSelectedSlotIndexChanged += VisulizeSelectedSlot;

        backgroundImage = GetComponent<Image>();
        backgroundImage.sprite = unselectedSlotSprite;
        inventoryItemUI.Init(slot);

        canvas = GetComponentInParent<Canvas>();
        draggedItemTransform = inventoryItemUI.GetComponent<RectTransform>();
        canvasGroup = inventoryItemUI.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = inventoryItemUI.gameObject.AddComponent<CanvasGroup>();
        }
    }
    void OnDestroy()
    {
        inventory.OnSelectedSlotIndexChanged -= VisulizeSelectedSlot;
    }
    private void VisulizeSelectedSlot()
    {
        backgroundImage.sprite = inventory.SelectedInventorySlotIndex == slotIndex ? selectedSlotSprite : unselectedSlotSprite;  
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventorySlot == null || InventorySlot.IsEmpty()) return;

        currentlyDraggedSlot = this;

        originalParent = draggedItemTransform.parent;

        draggedItemTransform.SetParent(canvas.transform, true);
        draggedItemTransform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (InventorySlot == null || InventorySlot.IsEmpty()) return;

        draggedItemTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItemTransform == null || canvasGroup == null) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (originalParent != null)
        {
            draggedItemTransform.SetParent(originalParent, false);
            draggedItemTransform.anchoredPosition = Vector2.zero;
        }

        currentlyDraggedSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (currentlyDraggedSlot == null || currentlyDraggedSlot == this) return;
        if (InventorySlot == null) return;

        var draggedSlot = currentlyDraggedSlot.InventorySlot;

        if (inventory != null && draggedSlot != null)
        {
            inventory.SwapInventorySlot(InventorySlot, draggedSlot);
        }

        Debug.Log($"Inventory: {inventory}");
        Debug.Log($"Target InventorySlot: {InventorySlot}");
        Debug.Log($"Dragged InventorySlot: {draggedSlot}");
    }


}