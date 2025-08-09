using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class explosionplate : MovePlate
{
    public override void Update()
    {
        NetworkObject refObj;
        if (!reference.Value.TryGet(out refObj))
        {
            Debug.LogError("Failed to resolve NetworkObjectReference in MovePlate.OnMouseUp!");
            return;
        }
        Chessman cmRef = refObj.GetComponent<Chessman>();
        NetworkObject cp = controller.GetComponent<Game>().GetPos(matrixX, matrixY);
        if (cp != null && cp.GetComponent<Chessman>().variant != null)
        {
            controller.GetComponent<Game>().APloseServerRpc(cp.GetComponent<Chessman>().player.Value.ToString(), cmRef.variant.PV - (cp.GetComponent<Chessman>().variant.PV / 10));
            APLoseThenDestroy(2f);
        }
        else
        {
            Debug.Log("Target dont have variant");
            Destroy(gameObject);
        }
    }

    private IEnumerator APLoseThenDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}