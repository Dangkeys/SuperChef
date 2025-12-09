using UnityEngine;

[CreateAssetMenu(fileName = "CookingRecipeSO", menuName = "Scriptable Objects/CookingRecipeSO")]
public class CookingRecipeSO : ScriptableObject
{
    [field: SerializeField] public FoodItem InputFoodItem { get; private set; }
    [field: SerializeField] public FoodItem OutputFoodItem { get; private set; }
    [field: SerializeField] public CookingType CookingType { get; private set; } = CookingType.Heating;

}