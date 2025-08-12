using Unity.Netcode;
using UnityEngine;

public class commitsuicide : NetworkBehaviour
{
    public float lifetime = 3f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Invoke(nameof(DespawnSelf), lifetime);
        }
    }

    void DespawnSelf()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
