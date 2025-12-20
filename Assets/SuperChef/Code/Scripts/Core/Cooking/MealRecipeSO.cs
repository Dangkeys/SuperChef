using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MealRecipeSO", menuName = "Scriptable Objects/MealRecipeSO")]
public class MealRecipeSO : BaseSO
{
    [field: SerializeField] public List<RecipeIngredient> FoodIngredients { get; private set; }

}
