using Unity.Netcode;

public class NetcodeHelper : NetworkBehaviour
{
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void DespawnServerRpc(NetworkObjectReference objRef)
    {
        if (!objRef.TryGet(out NetworkObject netObj)) return;
        if (netObj != null && netObj.IsSpawned)
        {
            netObj.Despawn(true);
        }
    }
}