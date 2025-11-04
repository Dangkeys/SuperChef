using QFSW.QC;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [AssetsOnly]
    [field: SerializeField] GameObject pickableObjectPrefab;

    [Command]
    public void SpawnPickableObject()
    {
        GameObject pickableObjectInstance =  Instantiate(pickableObjectPrefab);
        NetworkObject networkObject = pickableObjectInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }
    
}
