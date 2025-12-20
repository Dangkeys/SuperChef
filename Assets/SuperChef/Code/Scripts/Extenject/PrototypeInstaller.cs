using UnityEngine;
using Zenject;

public class PrototypeInstaller : MonoInstaller
{
    [SerializeField] private InventoryItemProviderSO inventoryItemProviderSO;
    [SerializeField] private CookingRecipeProviderSO cookingRecipeProviderSO;
    [SerializeField] private InventoryHelper inventoryHelper;
    [SerializeField] private NetcodeHelper netcodeHelper;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.Bind<InventoryHelper>().FromInstance(inventoryHelper).AsSingle().NonLazy();
        Container.Bind<NetcodeHelper>().FromInstance(netcodeHelper).AsSingle().NonLazy();
        Container.Bind<InventoryItemProviderSO>()
                    .FromInstance(inventoryItemProviderSO)
                    .AsSingle();
        Container.Bind<CookingRecipeProviderSO>()
                    .FromInstance(cookingRecipeProviderSO)
                    .AsSingle();
        Container.DeclareSignal<UIOpenSignal>();
        Container.DeclareSignal<PlayerSpawnedSignal>();
        Container.Bind<InputSignalReceiver>().AsSingle().NonLazy();
    }
}