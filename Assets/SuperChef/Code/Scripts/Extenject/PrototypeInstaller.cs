using UnityEngine;
using Zenject;

public class PrototypeInstaller : MonoInstaller
{
    [SerializeField] private InventoryItemProvider inventoryItemProviderPrefab;

    public override void InstallBindings()
    {
        Container.Bind<InventoryItemProvider>()
            .FromComponentInNewPrefab(inventoryItemProviderPrefab)
            .AsSingle().NonLazy();

    }
}