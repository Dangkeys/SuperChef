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
        Container.DeclareSignal<UIOpenSignal>();
        Container.DeclareSignal<PlayerSpawnedSignal>();
        Container.Bind<InputSignalReceiver>().AsSingle().NonLazy();
    }
}