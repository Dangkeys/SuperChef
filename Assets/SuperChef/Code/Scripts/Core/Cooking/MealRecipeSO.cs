using UnityEngine;

[CreateAssetMenu(fileName = "MealRecipeSO", menuName = "Scriptable Objects/MealRecipeSO")]
public class MealRecipeSO : BaseSO
{
    [field: SerializeField] public FoodItemSOAndCount[] FoodIngredients { get; private set; }

}

[System.Serializable]
public class FoodItemSOAndCount

{
    public FoodItemSO FoodItemSO;
    public int Count;
    public FoodItemSOAndCount(FoodItemSO foodItemSO, int count)
    {
        FoodItemSO = foodItemSO;
        Count = count;
    }
}