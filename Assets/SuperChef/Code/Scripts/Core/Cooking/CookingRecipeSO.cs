using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CookingRecipeSO", menuName = "Scriptable Objects/CookingRecipeSO")]
public class CookingRecipeSO : BaseSO
{
    [field: SerializeField] public List<RecipeIngredient> InputIngredientList { get; private set; }
    [field: SerializeField] public List<RecipeIngredient> OutputIngredient { get; private set; }
    [field: SerializeField] public CookingType Type { get; private set; } = CookingType.Chopping;
    [field: SerializeField] public int ChopCount { get; private set; } = 5;
    [field: SerializeField] public float HeatingDuration { get; private set; } = 5f;
    [field: SerializeField] public float GrillingDuration { get; private set; } = 10f;
    [field: SerializeField] public float StewingDuration { get; private set; } = 15f;
}
