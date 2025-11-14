using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class InGameUI : MonoBehaviour
{
    private SignalBus signalBus;

    [SerializeField] private GameObject crossHairGameObject;
    void Start()
    {
        crossHairGameObject.SetActive(false);
    }
    [Inject]
    public void Construct(SignalBus signalBus, GameInputReader gameInputReader)
    {
        this.signalBus = signalBus;
        signalBus.Subscribe<UIOpenSignal>(OnOpenUI);
        signalBus.Subscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
    }

    private void OnPlayerSpawned()
    {
        Cursor.lockState = CursorLockMode.Locked;
        crossHairGameObject.SetActive(true);
    }

    private void OnOpenUI(UIOpenSignal signal)
    {
        crossHairGameObject.SetActive(!signal.IsOpen);
        Cursor.lockState = signal.IsOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void OnDestroy()
    {
        signalBus.TryUnsubscribe<UIOpenSignal>(OnOpenUI);
    }



}