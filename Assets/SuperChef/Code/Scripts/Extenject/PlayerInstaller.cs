using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private PickUp pickUp;
    [SerializeField] private Inventory inventory;
    [SerializeField] private BuildingManager buildingManager;
    public override void InstallBindings()
    {
        Container.Bind<PickUp>().FromInstance(pickUp).AsSingle().NonLazy();
        Container.Bind<Inventory>().FromInstance(inventory).AsSingle().NonLazy();
        Container.Bind<BuildingManager>().FromInstance(buildingManager).AsSingle().NonLazy();
    }
}