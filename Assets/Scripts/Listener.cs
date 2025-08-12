using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class Listener : MonoBehaviour
{
    public Player player;
    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.OnServerStopped += OnServerStopped;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // If the disconnected client is yourself
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            StartCoroutine(CallDropFade());
            Debug.Log("We were disconnected from the host.");
        }
    }

    private void OnServerStopped(bool _)
    {
        StartCoroutine(CallDropFade());
        Debug.Log("Host server has stopped!");
    }
    IEnumerator CallDropFade()
    {
        string fullUrl = $"http://localhost/testdating/dropafade.php?user_id={player.data.user_id}";
        using (UnityWebRequest www = UnityWebRequest.Get(fullUrl))
        {
            // Send the request
            yield return www.SendWebRequest();

            // Check for errors
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // Get the JSON response
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);
            }
        }
    }
}
