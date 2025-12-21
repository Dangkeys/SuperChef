using UnityEngine;
using Zenject;

public class AutoInjectOnAwake : MonoBehaviour
{
    void Awake()
    {
        // ProjectContext.Instance.Container.InjectGameObject(gameObject);
        
        SceneContext sceneContext = FindFirstObjectByType<SceneContext>();
        sceneContext?.Container.InjectGameObject(gameObject);
    }
}