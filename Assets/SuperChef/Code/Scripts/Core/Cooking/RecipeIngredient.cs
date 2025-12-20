using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class RecipeIngredient
{
    private const int DEFAULT_AMOUNT = 1;
    [ShowInInspector, HideLabel]
    private Sprite PreviewSO => FoodItemSO?.ItemSprite;
    [field: SerializeField] public FoodItemSO FoodItemSO { get; private set; }
    [field: SerializeField] public int Amount { get; private set; } = DEFAULT_AMOUNT;
    public RecipeIngredient(FoodItemSO foodItemSO, int count = DEFAULT_AMOUNT)
    {
        FoodItemSO = foodItemSO;
        Amount = count;   
    }
}
