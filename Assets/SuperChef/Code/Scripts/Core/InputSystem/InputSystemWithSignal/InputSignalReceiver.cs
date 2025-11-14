using System;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputSignalReceiver
{
    public InputActions InputActions { get; private set; }
    private GameInputReader inputReader;
    private SignalBus signalBus;

    [Inject]
    private void Init(InputActions inputActions,SignalBus signalBus, GameInputReader gameInputReader)
    {
        inputReader = gameInputReader;
        InputActions = inputActions;
        this.signalBus = signalBus;
        if (signalBus == null) return;
        signalBus.Subscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
        signalBus.Subscribe<UIOpenSignal>(OnUIOpened);
    }

    private void OnUIOpened(UIOpenSignal signal)
    {
        inputReader.EnableInputActionMap(signal.IsOpen ? InputActions.UI : InputActions.Player);
    }

    private void OnPlayerSpawned(PlayerSpawnedSignal signal)
    {
        inputReader.EnableInputActionMap(InputActions.Player);
    }
}
