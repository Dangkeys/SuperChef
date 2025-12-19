using UnityEngine;

[CreateAssetMenu(fileName = "CookingRecipeSO", menuName = "Scriptable Objects/CookingRecipeSO")]
public class CookingRecipeSO : ScriptableObject
{
    [field: SerializeField] public FoodItemSO InputFoodItemSO { get; private set; }
    [field: SerializeField] public FoodItemSO OutputFoodItemSO { get; private set; }
    [field: SerializeField] public CookingType CookingType { get; private set; } = CookingType.Heating;
}