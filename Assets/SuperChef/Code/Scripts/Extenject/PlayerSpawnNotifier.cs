using Unity.Netcode;
using UnityEngine;
using Zenject;

public class PlayerSpawnNotifier : NetworkBehaviour
{
    private Inventory inventory;
    private SignalBus signalBus;

    [Inject]
    public void Construct(Inventory inventory, SignalBus signalBus)
    {
        this.inventory = inventory;
        this.signalBus = signalBus;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // Fire the signal
        Debug.Log("Fire player spawned");
        signalBus.Fire(new PlayerSpawnedSignal { Inventory = inventory });
    }
}
