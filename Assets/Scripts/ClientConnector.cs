using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ClientConnector : MonoBehaviour
{
    public string serverIP = "192.168.1.15";
    public ushort serverPort = 7777;

    public void ConnectToHost()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            // Sets the remote server endpoint AND local bind address in one call
            // "0.0.0.0" means "bind to any local address", and port=0 lets the OS pick a free one
            transport.SetConnectionData(serverIP, serverPort, "0.0.0.0");
        }
        else
        {
            Debug.LogError("UnityTransport not found on NetworkManager.");
            return;
        }

        NetworkManager.Singleton.StartClient();
    }
}
