using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
public class BootstrapInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var message = "Hello World!, This is BootstrapInstaller";
        Debug.Log(message);
    }
}