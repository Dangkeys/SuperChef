using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CookingRecipeProviderSO", menuName = "Scriptable Objects/CookingRecipeProviderSO")]
public class CookingRecipeProviderSO : ScriptableObject
{

    [field: SerializeField] public List<CookingRecipeSO> CookingRecipeSOList { get; private set; }


    public CookingRecipeSO GetRecipeSOFromInputAndCookingType(List<RecipeIngredient> inputFoodStackSOList, CookingType cookingType)
    {
        foreach (CookingRecipeSO cookingRecipeSO in CookingRecipeSOList)
        {
            if (AreFoodStackListsEqual(cookingRecipeSO.InputIngredientList, inputFoodStackSOList) && cookingRecipeSO.Type == cookingType)
            {
                return cookingRecipeSO;
            }
        }
        return null;
    }
    private bool AreFoodStackListsEqual(List<RecipeIngredient> list1, List<RecipeIngredient> list2)
    {
        if (list1 == null || list2 == null)
            return list1 == list2;

        if (list1.Count != list2.Count)
            return false;

        // Check if all items in list1 exist in list2 with same amounts
        foreach (var recipe1 in list1)
        {
            bool found = list2.Any(recipe2 =>
                recipe2.FoodItemSO == recipe1.FoodItemSO &&
                recipe2.Amount == recipe1.Amount);

            if (!found)
                return false;
        }
        return true;
    }
}