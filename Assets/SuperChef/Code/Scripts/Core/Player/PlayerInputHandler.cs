using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class PlayerInputHandler : NetworkBehaviour
{
    private GameInputReader inputReader;
    private PickUp pickUp;
    private Inventory inventory;
    private PlayerAnimationHandler playerAnimationHandler;
    private CookingManager cookingHandler;
    private BuildingManager buildingManager;

    [Inject]
    private void Init(GameInputReader inputReader, PickUp pickUp, Inventory inventory, PlayerAnimationHandler playerAnimationHandler, BuildingManager buildingManager, CookingManager cookingHandler)
    {
        this.inputReader = inputReader;
        this.pickUp = pickUp;
        this.inventory = inventory;
        this.playerAnimationHandler = playerAnimationHandler;
        this.buildingManager = buildingManager;
        this.cookingHandler = cookingHandler;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent += OnInteractPressed;
        inputReader.AttackEvent += OnAttackPressed;
    }

    private void OnAttackPressed()
    {
        if(buildingManager.OnTryBuildObject()) return;
        
        cookingHandler.PerformInteraction();
        playerAnimationHandler.TriggerAttackAnimation();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.InteractEvent -= OnInteractPressed;
        inputReader.AttackEvent -= OnAttackPressed;
    }

    private void OnInteractPressed()
    {

        if (pickUp.PerformInteraction()) return;


        inventory.PerformInteraction();
    }
}