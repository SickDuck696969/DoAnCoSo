using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;
using TMPro;
using Unity.Collections;
using Unity.Multiplayer.Playmode;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.WSA;
using static UnityEditor.UIElements.ToolbarMenu;

public class MovePlate : NetworkBehaviour
{
    public GameObject controller;
    public NetworkVariable<NetworkObjectReference> reference = new NetworkVariable<NetworkObjectReference>();
    public int matrixX;
    public int matrixY;
    public NetworkVariable<bool> attack =
        new NetworkVariable<bool>(false);
    public NetworkVariable<bool> esploding =
        new NetworkVariable<bool>(false);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        controller = GameObject.FindGameObjectWithTag("GameController");
    }

    [ServerRpc(RequireOwnership = false)]
    public void AttackServerRpc()
    {
        Game sc = controller.GetComponent<Game>();
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

    public virtual void OnMouseUp()
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
                rook.GetComponent<Chessman>().hasMoved.Value = true;
            }
            // Queen-side
            else if (matrixX == 2)
            {
                NetworkObject rook = controller.GetComponent<Game>().GetPos(0, matrixY);
                controller.GetComponent<Game>().SetPosEmptyServerRpc(0, matrixY);
                rook.GetComponent<Chessman>().SetXBoardServerRpc(3);
                rook.GetComponent<Chessman>().SetCoords();
                controller.GetComponent<Game>().SetPos(rook);
                rook.GetComponent<Chessman>().hasMoved.Value = true;
            }
        }

        //move piece
        MovePieceServerRpc(startX, startY);
        controller.GetComponent<Game>().positions[cmRef.GetXBoard(), cmRef.GetYBoard()] = refObj;
        if (controller == null)
        {
            Debug.LogError("MovePlate.controller is null!");
            return;
        }
        if (cmRef.variant != null)
        {
            if (!cmRef.hasSpelled.Value)
            {
                Debug.Log("aint spelled yet");
                cmRef.SetButton();
            }
        }
        else cmRef.changeturnServerRpc();
        cmRef.DestroyMovePlatesServerRpc();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMoveSFX();
        }
        Debug.Log(controller.GetComponent<Game>().currentColor.data.username + " moved " + cmRef.name + " to [" + matrixX + " " + matrixY + " ]");
    }
    [ServerRpc(RequireOwnership = false)]
    public void clearingServerRpc(int startX, int startY)
    {
        controller.GetComponent<Game>().SetPosEmptyServerRpc(startX, startY);
    }
    [ServerRpc(RequireOwnership = false)]
    public void MovePieceServerRpc(int startX, int startY)
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
        cmRef.hasMoved.Value = true;
        cmRef.xmodifier.Value = ((matrixX - startX) > 0) ? 1 : -1;
        cmRef.ymodifier.Value = ((matrixY - startY) > 0) ? 1 : -1;
        if (esploding.Value)
        {
            for (int i = matrixX - 1; i <= matrixX + 1; i++)
            {
                for (int y = matrixY - 1; y <= matrixY + 1; y++)
                {
                    if (!(i == matrixX && y == matrixY))
                    {
                        cmRef.EffectSpawnServerRpc(1, i, y);
                    }
                }
            }
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

    public virtual void Update()
    {

    }
}
