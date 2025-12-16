using UnityEngine;
using Zenject;

public class PrototypeInstaller : MonoInstaller
{
    [SerializeField] private InventoryItemProviderSO inventoryItemProviderSO;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.Bind<InventoryItemProviderSO>()
                    .FromInstance(inventoryItemProviderSO)
                    .AsSingle();
        Container.DeclareSignal<UIOpenSignal>();
        Container.DeclareSignal<PlayerSpawnedSignal>();
        Container.Bind<InputSignalReceiver>().AsSingle().NonLazy();
    }
}