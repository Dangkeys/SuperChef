using QFSW.QC;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [AssetsOnly]
    [SerializeField] private GameObject pickableObjectPrefab;

    [Command]
    public void SpawnPickableObject()
    {
        GameObject pickableObjectInstance = Instantiate(pickableObjectPrefab, new Vector3(4, 2, 4), Quaternion.identity);
        NetworkObject networkObject = pickableObjectInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }
    
    
}
