using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CookingRecipeProviderSO", menuName = "Scriptable Objects/CookingRecipeProviderSO")]
public class CookingRecipeProviderSO : ScriptableObject {
    [field: SerializeField] public List<CookingRecipeSO> CookingRecipeSOList { get; private set; }


    public FoodItemSO GetOutputFromInputAndCookingType(FoodItemSO inputFoodItemSO, CookingType cookingType)
    {
        foreach(CookingRecipeSO cookingRecipeSO in CookingRecipeSOList)
        {
            if(cookingRecipeSO.InputFoodItemSO == inputFoodItemSO &&  cookingRecipeSO.CookingType == cookingType)
            {
                return cookingRecipeSO.OutputFoodItemSO;
            }
        }
        return null;
    }
}