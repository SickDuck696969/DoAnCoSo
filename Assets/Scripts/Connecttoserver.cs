using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Connecttoserver : MonoBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button client;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        host.onClick.AddListener(hostclick);
        client.onClick.AddListener(clientclick);
    }

    private void hostclick()
    {
        Debug.Log("hosting");
        NetworkManager.Singleton.StartHost();
    }

    private void clientclick()
    {
        Debug.Log("joining");
        NetworkManager.Singleton.StartClient();
    }
}
