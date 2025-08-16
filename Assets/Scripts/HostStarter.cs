using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostStarter : MonoBehaviour
{
    public ushort listenPort = 7777;

    public void StartHost()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.SetConnectionData("0.0.0.0", listenPort);
        }
        else
        {
            Debug.LogError("UnityTransport not found on NetworkManager.");
            return;
        }
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        if (!NetworkManager.Singleton.StartHost())
        {
            Debug.LogError("Failed to start host!");
        }
        else
        {
            string ip = transport.ConnectionData.Address;
            ushort port = transport.ConnectionData.Port;
            Debug.Log($"[HostInfo] Host listening on {ip}:{port}");
        }
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Local host client connected.");
            SceneManager.LoadScene("VarianSelection", LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Remote client " + clientId + " connected.");
            NetworkManager.Singleton.SceneManager.LoadScene(
                "VarianSelection",
                LoadSceneMode.Single
            );
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log("Client " + clientId + " disconnected.");
    }
}
