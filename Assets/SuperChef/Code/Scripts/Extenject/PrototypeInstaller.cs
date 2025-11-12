using UnityEngine;
using Zenject;

public class PrototypeInstaller : MonoInstaller
{
    [SerializeField] private InventoryItemProvider inventoryItemProviderPrefab;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.Bind<InventoryItemProvider>()
            .FromComponentInNewPrefab(inventoryItemProviderPrefab)
            .AsSingle().NonLazy();
        Container.DeclareSignal<PlayerSpawnedSignal>();
    }
}