using UnityEngine;
using Zenject; // Extenject uses the Zenject namespace

public class AutoInjectOnAwake : MonoBehaviour
{
    void Awake()
    {
        // Inject all components on this prefab instance
        ProjectContext.Instance.Container.InjectGameObject(gameObject);
    }
}
