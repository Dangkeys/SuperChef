using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseSO : ScriptableObject
{
    [field: SerializeField, ReadOnly] public string ID { get; private set; }

    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = Guid.NewGuid().ToString();
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}