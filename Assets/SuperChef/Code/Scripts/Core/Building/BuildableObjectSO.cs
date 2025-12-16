using UnityEngine;

[CreateAssetMenu(fileName = "BuildableObjectSO", menuName = "Scriptable Objects/BuildableObjectSO")]
public class BuildableObjectSO : HoldableItemSO
{
    [field: SerializeField] public BuildableObject BuildableObject { get; private set; }
    [field: SerializeField] public GameObject BuildableObjectGhostPrefab { get; private set; }
}
