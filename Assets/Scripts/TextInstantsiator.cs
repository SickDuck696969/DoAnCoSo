using TMPro;
using UnityEngine;
using Unity.Netcode;

public class TextInstantiator : NetworkBehaviour
{
    public GameObject messagePrefab;
    public Player player;

    void Update()
    {
        if (!IsClient) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            string message = player.data.username + ": " + GetComponent<TMP_InputField>().text;
            SubmitMessageServerRpc(message);
            GetComponent<TMP_InputField>().text = "";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SubmitMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        // Tell all clients to create the object locally
        SpawnMessageClientRpc(message);
    }

    [ClientRpc]
    void SpawnMessageClientRpc(string message, ClientRpcParams rpcParams = default)
    {
        GameObject msg = Instantiate(messagePrefab, this.transform.parent.Find("Viewport/Content"));
        TMP_Text tmp = msg.GetComponentInChildren<TMP_Text>();
        tmp.text = message;
    }
}
