using System;
using UnityEngine;

[Serializable]
public class FoodContainer
{
    [field: SerializeField] public FoodItemSO FoodItemSO { get; private set; }
    [field: SerializeField] public int CurrentAmount { get; private set; } = 0;
}
