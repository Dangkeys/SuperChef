using Unity.Netcode;
using UnityEngine;
using Zenject;

public class PlayerInteractionHandler : NetworkBehaviour
{
    private GameInputReader inputReader;
    private PickUp pickUp;
    private Inventory inventory;

    [Inject]
    private void Init(GameInputReader inputReader, PickUp pickUp, Inventory inventory)
    {
        this.inputReader = inputReader;
        this.pickUp = pickUp;
        this.inventory = inventory;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent += OnInteractPressed;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent -= OnInteractPressed;
    }

    private void OnInteractPressed()
    {

        if (pickUp.PerformInteraction()) return;


        inventory.PerformInteraction();
    }
}