using System.Drawing;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;

public class MovePlate : NetworkBehaviour
{
    GameObject controller;
    public NetworkVariable<NetworkObjectReference> reference = new NetworkVariable<NetworkObjectReference>();
    int matrixX;
    int matrixY;
    public NetworkVariable<bool> attack =
        new NetworkVariable<bool>(false);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        controller = GameObject.FindGameObjectWithTag("GameController");
    }

    [ServerRpc(RequireOwnership = false)]
    public void AttackServerRpc()
    {
        NetworkObject refObj;
        if (!reference.Value.TryGet(out refObj))
        {
            Debug.LogError("Failed to resolve NetworkObjectReference in MovePlate.OnMouseUp!");
            return;
        }
        Chessman cmRef = refObj.GetComponent<Chessman>();
        NetworkObject cp = controller.GetComponent<Game>().GetPos(matrixX, matrixY);
        if (cp.name == "bl_king" || cp.name == "wh_king")
        {
            controller.GetComponent<Game>().Winner(cmRef.player.Value.ToString());
        }
        cp.GetComponent<NetworkObject>().Despawn();
    }

    public void OnMouseUp()
    {
        NetworkObject refObj;
        if (!reference.Value.TryGet(out refObj))
        {
            Debug.LogError("Failed to resolve NetworkObjectReference in MovePlate.OnMouseUp!");
            return;
        }

        Chessman cmRef = refObj.GetComponent<Chessman>();
        int startX = cmRef.GetXBoard();
        int startY = cmRef.GetYBoard();

        if (attack.Value)
        {
            AttackServerRpc();
        }

        // Castling logic BEFORE moving the king
        if ((cmRef.name == "wh_king" || cmRef.name == "bl_king") && Mathf.Abs(matrixX - startX) == 2)
        {
            // King-side
            if (matrixX == 6)
            {
                NetworkObject rook = controller.GetComponent<Game>().GetPos(7, matrixY);
                controller.GetComponent<Game>().SetPosEmptyServerRpc(7, matrixY);
                rook.GetComponent<Chessman>().SetXBoardServerRpc(5);
                rook.GetComponent<Chessman>().SetCoords();
                controller.GetComponent<Game>().SetPos(rook);
                rook.GetComponent<Chessman>().hasMoved = true;
            }
            // Queen-side
            else if (matrixX == 2)
            {
                NetworkObject rook = controller.GetComponent<Game>().GetPos(0, matrixY);
                controller.GetComponent<Game>().SetPosEmptyServerRpc(0, matrixY);
                rook.GetComponent<Chessman>().SetXBoardServerRpc(3);
                rook.GetComponent<Chessman>().SetCoords();
                controller.GetComponent<Game>().SetPos(rook);
                rook.GetComponent<Chessman>().hasMoved = true;
            }
        }

        // Clear old position
        if (controller == null)
        {
            Debug.LogError("MovePlate.controller is null!");
            return;
        }
        controller.GetComponent<Game>().SetPosEmptyServerRpc(startX, startY);

        //move piece
        MovePieceServerRpc();
        controller.GetComponent<Game>().positions[cmRef.GetXBoard(), cmRef.GetYBoard()] = refObj;
        cmRef.DestroyMovePlatesServerRpc();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMoveSFX();
        }
        Debug.Log(controller.GetComponent<Game>().currentColor.data.username + " moved " + cmRef.name + " to [" + matrixX + " " + matrixY + " ]");
    }
    [ServerRpc(RequireOwnership = false)]
    public void MovePieceServerRpc()
    {
        NetworkObject refObj;
        if (!reference.Value.TryGet(out refObj))
        {
            Debug.LogError("Failed to resolve NetworkObjectReference in MovePlate.OnMouseUp!");
            return;
        }

        Chessman cmRef = refObj.GetComponent<Chessman>();
        cmRef.SetXBoardServerRpc(matrixX);
        cmRef.SetYBoardServerRpc(matrixY);
        cmRef.GetYBoard();
        cmRef.SetCoords();
        controller.GetComponent<Game>().SetPos(refObj);
        cmRef.hasMoved = true;
        if (IsServer)
        {
            controller.GetComponent<Game>().currentPlayer.Value = (controller.GetComponent<Game>().currentPlayer.Value.ToString() == "White") ? "Black" : "White";
        }
    }

    [ClientRpc]
    public void SetColorClientRpc(UnityEngine.Color newColor)
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = newColor;
        }
        else Debug.Log("banh mi");
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(NetworkObject obj)
    {
        reference.Value = obj;
    }

    public NetworkObject GetReference()
    {
        return reference.Value;
    }
}
