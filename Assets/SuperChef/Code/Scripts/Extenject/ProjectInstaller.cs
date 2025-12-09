using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private SettingsHandler settingsHandler;
    public override void InstallBindings()
    {
        // Example: services that should auto-run
        Container.BindInterfacesTo<GameBootFlow>().AsSingle(); // IInitializable/ITickable/etc.
        Container.Bind<GameInputReader>().AsSingle().NonLazy();
        Container.Bind<InputActions>().AsSingle().NonLazy();
        Container.Bind<SettingsHandler>().FromInstance(settingsHandler).AsSingle().NonLazy();
    }
}


// Auto-running service (optional)
public class GameBootFlow : IInitializable, System.IDisposable, ITickable
{
    private bool _initialized;

    public async void Initialize()
    {
#if UNITY_EDITOR
        // Skip Unity Services when running in EditMode or Tests
        if (UnityEngine.Application.isEditor && !UnityEngine.Application.isPlaying)
        {
            Debug.Log("[GameBootFlow] Skipped UnityServices initialization in editor/test mode.");
            return;
        }
#endif

        // Prevent re-initialization
        if (_initialized)
            return;

        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            await SignUpAnonymouslyIfNeeded();
            _initialized = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[GameBootFlow] Initialization failed: {ex.GetType().Name} - {ex.Message}");
        }
    }

    public void Tick() { /* runs every frame */ }
    public void Dispose()
    {
        try
        {
            // Skip if Unity Services not initialized
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.Log("[GameBootFlow] Dispose skipped (UnityServices not initialized).");
                return;
            }

            // Sign out only if actually signed in
            if (AuthenticationService.Instance?.IsSignedIn ?? false)
            {
                AuthenticationService.Instance.SignOut();
                Debug.Log("[GameBootFlow] Signed out on dispose.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[GameBootFlow] Dispose skipped due to: {ex.GetType().Name} - {ex.Message}");
        }
    }

    private async Task SignUpAnonymouslyIfNeeded()
    {
        if (AuthenticationService.Instance?.IsSignedIn ?? false)
        {
            Debug.Log("[GameBootFlow] Already signed in.");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"[GameBootFlow] Signed in anonymously! PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogWarning($"[GameBootFlow] Auth failed: {ex.Message}");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogWarning($"[GameBootFlow] Request failed: {ex.Message}");
        }
    }
}
