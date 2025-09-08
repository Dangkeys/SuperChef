using UnityEngine;
using Zenject; // Extenject uses the Zenject namespace

public class AutoInjectOnAwake : MonoBehaviour
{
    void Awake()
    {
        ProjectContext.Instance.Container.InjectGameObject(gameObject);
    }
}
