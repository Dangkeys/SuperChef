using UnityEngine;
using Zenject;

public class PrototypeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Container.Bind<NetcodeManager>().AsSingle().NonLazy();
    }
}