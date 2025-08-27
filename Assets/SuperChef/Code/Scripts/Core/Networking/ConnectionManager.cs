using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [Command]
    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    [Command]
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
