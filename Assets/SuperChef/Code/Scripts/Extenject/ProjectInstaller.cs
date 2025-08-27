using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Non-MonoBehaviour singletons
        Container.Bind<SaveService>().AsSingle();

        // MonoBehaviour singletons created on a new GameObject (persist via ProjectContext)
        Container.BindInterfacesAndSelfTo<AudioManager>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();

        // Example: services that should auto-run
        Container.BindInterfacesTo<GameBootFlow>().AsSingle(); // IInitializable/ITickable/etc.
        Container.Bind<GameInputReader>().AsSingle().NonLazy();
        Container.Bind<InputActions>().AsSingle().NonLazy();
    }
}

// Sample services
public class SaveService { /* ... */ }

public class AudioManager : MonoBehaviour{ /* ... */ }

// Auto-running service (optional)
public class GameBootFlow : IInitializable, System.IDisposable, ITickable
{
    public async void Initialize()
    {
        await UnityServices.InitializeAsync();
        await SignUpAnonymouslyAsync();
        // SceneManager.LoadScene("TestScene");
    }
    public void Tick() { /* runs every frame */ }
    public void Dispose() { /* runs on app quit */ }
    async Task SignUpAnonymouslyAsync()
{
    try
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Sign in anonymously succeeded!");

        // Shows how to get the playerID
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

    }
    catch (AuthenticationException ex)
    {
        // Compare error code to AuthenticationErrorCodes
        // Notify the player with the proper error message
        Debug.LogException(ex);
    }
    catch (RequestFailedException ex)
    {
        // Compare error code to CommonErrorCodes
        // Notify the player with the proper error message
        Debug.LogException(ex);
     }
}
}
