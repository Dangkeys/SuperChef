using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class ChoppingHandler : NetworkBehaviour
{
    private const CookingType cookingType = CookingType.Chopping;
    public RecipeIngredient InputIngredient { get; private set; }
    private CookingRecipeProviderSO cookingRecipeProviderSO;
    private NetcodeHelper netcodeHelper;
    private InventoryHelper inventoryHelper;

    public CookingRecipeSO CurrentRecipeSO { get; private set; }
    public NetworkVariable<int> CurrentCount { get; private set; } = new NetworkVariable<int>(0);

    [Inject]
    private void Init(CookingRecipeProviderSO cookingRecipeProviderSO, NetcodeHelper netcodeHelper, InventoryHelper inventoryHelper)
    {
        this.cookingRecipeProviderSO = cookingRecipeProviderSO;
        this.netcodeHelper = netcodeHelper;
        this.inventoryHelper = inventoryHelper;
        SetupRecipeIngredient();
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestToCutServerRpc()
    {
        // Prevent race conditions by checking if already complete
        if (CurrentCount.Value >= CurrentRecipeSO.ChopCount)
        {
            return;
        }

        CurrentCount.Value++;
        if (CurrentCount.Value >= CurrentRecipeSO.ChopCount)
        {
            SpawnOutputIngredientList();
            netcodeHelper.DespawnServerRpc(new NetworkObjectReference(NetworkObject));
        }
    }
    private void SpawnOutputIngredientList()
    {
        foreach (RecipeIngredient recipeIngredient in CurrentRecipeSO.OutputIngredient)
        {
            for (int i = 0; i < recipeIngredient.Amount; i++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-.25f, .25f),
                    Random.Range(-1f, 0f),
                    Random.Range(-.25f, .25f)
                );
                Vector3 spawnPos = transform.position + transform.up * 1f + randomOffset;
                inventoryHelper.RequestSpawnInventoryItemServerRpc(recipeIngredient.FoodItemSO.ID, spawnPos, Quaternion.identity);
            }
        }
    }

    private void SetupRecipeIngredient()
    {
        FoodItem foodItem = GetComponent<FoodItem>();
        FoodItemSO foodItemSO = foodItem.InventoryItemSO as FoodItemSO;
        if (foodItemSO == null) return;
        InputIngredient = new RecipeIngredient(foodItemSO);

        CurrentRecipeSO = cookingRecipeProviderSO.GetRecipeSOFromInputAndCookingType(new List<RecipeIngredient> { InputIngredient }, cookingType);

        if (CurrentRecipeSO == null) return;


    }
}