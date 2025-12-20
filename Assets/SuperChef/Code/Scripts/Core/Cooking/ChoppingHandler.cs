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

    public CookingRecipeSO CurrentRecipeSO {get; private set;} 
    public int CurrentCount { get; private set; } = 0;

    [Inject]
    private void Init(CookingRecipeProviderSO cookingRecipeProviderSO, NetcodeHelper netcodeHelper)
    {
        this.cookingRecipeProviderSO = cookingRecipeProviderSO;
        this.netcodeHelper = netcodeHelper;
    }
    void Awake()
    {
        SetupRecipeIngredient();
    }


    public void Cut()
    {
        CurrentCount++;
        if(CurrentCount >= CurrentRecipeSO.ChopCount)
        {
            
            netcodeHelper.DespawnServerRpc(new NetworkObjectReference(NetworkObject));
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