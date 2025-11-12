using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InventoryUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Inventory inventory;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        signalBus.Subscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(PlayerSpawnedSignal signal)
    {
        Initialize(signal.Inventory);
    }

    public void Initialize(Inventory playerInventory)
    {
        inventory = playerInventory;
    }

    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }
}
