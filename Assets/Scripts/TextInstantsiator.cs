using TMPro;
using UnityEngine;
using Unity.Netcode;

public class TextInstantiator : NetworkBehaviour
{
    public GameObject whitePrefab;

    void Update()
    {
        if (!IsClient) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            string message = GetComponent<TMP_InputField>().text;
            SubmitMessageServerRpc(message);
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
        // Each client finds their own blackParent by tag or other method
        Transform blackParent = GameObject.FindGameObjectWithTag("Black").transform;

        GameObject obj = Instantiate(whitePrefab, blackParent);
        obj.GetComponentInChildren<TMP_Text>().text = message;
    }
}
