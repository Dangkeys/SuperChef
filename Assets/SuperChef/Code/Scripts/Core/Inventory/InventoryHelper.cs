using Unity.Netcode;
using UnityEngine;
using Zenject;

public class InventoryHelper : NetworkBehaviour
{
    private InventoryItemProviderSO inventoryItemProviderSO;

    [Inject]
    private void Init(InventoryItemProviderSO inventoryItemProviderSO)
    {
        this.inventoryItemProviderSO = inventoryItemProviderSO;
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestSpawnInventoryItemServerRpc(string inventoryItemSOID, Vector3 position, Quaternion rotation, NetworkObjectReference parentRef = default, bool isKinematic = false)
    {
        SpawnObject(inventoryItemSOID, position, rotation, parentRef, isKinematic);
    }
    private void SpawnObject(string inventoryItemSOID, Vector3 position, Quaternion rotation, NetworkObjectReference parentRef = default, bool isKinematic = false)
    {
        var inventoryItemPrefab = inventoryItemProviderSO.GetInventoryItemByID(inventoryItemSOID);
        if (inventoryItemPrefab == null) return;
        var newObj = Instantiate(inventoryItemPrefab, position, rotation);
        newObj.NetworkObject.Spawn(true);

        if (parentRef.TryGet(out var parentNetworkObject))
        {
            newObj.NetworkObject.TrySetParent(parentNetworkObject.transform);
        }
        if (newObj.TryGetComponent(out InventoryItem inventoryItem))
        {
            inventoryItem.SetIsKinematic(isKinematic);
        }

    }
}