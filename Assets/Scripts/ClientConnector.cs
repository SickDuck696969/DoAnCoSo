using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ClientConnector : MonoBehaviour
{
    public string serverIP = "192.168.1.7";
    public ushort serverPort = 7777;

    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    public void ConnectToHost()
    {

        if (NetworkManager.Singleton.IsListening)
        {
            Debug.Log("[ClientInfo] Client already running, shutting it down...");
            NetworkManager.Singleton.Shutdown();

            if (isActiveAndEnabled)
            {
                StartCoroutine(RestartClientRoutine());
            }
            else
            {
                Debug.LogWarning("[ClientInfo] ClientConnector inactive, skipping coroutine restart.");
                StartNewClient();
            }
            return;
        }

        StartNewClient();
    }

    private void StartNewClient()
    {
        Debug.Log($"{serverIP} is connecting to host {serverPort}");

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.SetConnectionData(serverIP, serverPort, "0.0.0.0");
        }
        else
        {
            Debug.LogError("UnityTransport not found on NetworkManager.");
            return;
        }

        Debug.Log("Attempting to connect to server " + serverIP + ":" + serverPort);

        if (!NetworkManager.Singleton.StartClient())
        {
            Debug.LogError("Failed to start client!");
        }
    }

    private IEnumerator RestartClientRoutine()
    {
        yield return null;
        StartNewClient();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Successfully connected to server as ClientID: " + clientId);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogWarning("Disconnected from server.");
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}
