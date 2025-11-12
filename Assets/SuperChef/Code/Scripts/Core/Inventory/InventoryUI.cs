using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private InventorySlotUI currentInventorySlotUI;

    void Update()
    {
        if (currentInventorySlotUI == null) return;
        currentInventorySlotUI.transform.position = Input.mousePosition;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}