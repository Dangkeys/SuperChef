using System.Collections.Generic;
using Zenject;

public class InputSignalReceiver
{
    public Stack<InputActions> inputActionsStack = new Stack<InputActions>();
    public InputActions InputActions { get; private set; }
    private GameInputReader inputReader;
    private SignalBus signalBus;

    [Inject]
    private void Init(InputActions inputActions, SignalBus signalBus, GameInputReader gameInputReader)
    {
        inputReader = gameInputReader;
        InputActions = inputActions;
        this.signalBus = signalBus;
        if (signalBus == null) return;
        this.signalBus.Subscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
        this.signalBus.Subscribe<UIOpenSignal>(OnUIOpened);
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
