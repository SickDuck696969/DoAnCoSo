using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HostStarter : MonoBehaviour
{
    public ushort listenPort = 7777;
    private bool isRestarting = false;

    public void StartHost()
    {
        if (isRestarting) return;

        if (NetworkManager.Singleton.IsListening)
        {
            if (isActiveAndEnabled)
            {
                StartCoroutine(RestartHostRoutine());
            }
            else
            {
                Debug.LogWarning("[HostInfo] HostStarter inactive, skipping coroutine restart.");
            }
            return;
        }

        StartNewHost();
    }

    private void StartNewHost()
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
    }

    private IEnumerator RestartHostRoutine()
    {
        isRestarting = true;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

        NetworkManager.Singleton.Shutdown();

        yield return null;
        while (NetworkManager.Singleton.IsListening)
        {
            yield return null;
        }

        Debug.Log("[HostInfo] Restarting host...");
        StartNewHost();
        isRestarting = false;
    }

    public void ShutHost()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.Log("[HostInfo] Shutting down host...");

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

            NetworkManager.Singleton.Shutdown();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Local host client connected.");
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

    public void OnClientDisconnected(ulong clientId)
    {
        Debug.Log("Client " + clientId + " disconnected.");
    }
}
