using System;
using System.Collections;
using Unity.Collections;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.WSA;

public class Chessman : NetworkBehaviour
{
    public GameObject controller;
    public GameObject plate;
    public NetworkVariable<FixedString64Bytes> player =
    new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<FixedString64Bytes> piecename =
        new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<int> x =
        new NetworkVariable<int>();
    public NetworkVariable<int> y =
        new NetworkVariable<int>();
    private NetworkVariable<int> xBoard = new NetworkVariable<int>(-1);
    private NetworkVariable<int> yBoard = new NetworkVariable<int>(-1);
    public bool hasMoved = false;
    public Sprite bl_queen,
        bl_rook,
        bl_bishop,
        bl_knight,
        bl_pawn,
        bl_king,
        wh_queen,
        wh_rook,
        wh_bishop,
        wh_knight,
        wh_pawn,
        wh_king;

    public override void OnNetworkSpawn()
    {
        SetXBoardServerRpc(x.Value);
        SetYBoardServerRpc(y.Value);
        this.name = piecename.Value.ToString();
        Activate();
    }


    public void Activate()
    {
        switch (this.name)
        {
            case "bl_queen": this.GetComponent<SpriteRenderer>().sprite = bl_queen; break;
            case "bl_rook": this.GetComponent<SpriteRenderer>().sprite = bl_rook; break;
            case "bl_bishop": this.GetComponent<SpriteRenderer>().sprite = bl_bishop; break;
            case "bl_knight": this.GetComponent<SpriteRenderer>().sprite = bl_knight; break;
            case "bl_pawn": this.GetComponent<SpriteRenderer>().sprite = bl_pawn; break;
            case "bl_king": this.GetComponent<SpriteRenderer>().sprite = bl_king; break;
            case "wh_queen": this.GetComponent<SpriteRenderer>().sprite = wh_queen; break;
            case "wh_rook": this.GetComponent<SpriteRenderer>().sprite = wh_rook; break;
            case "wh_bishop": this.GetComponent<SpriteRenderer>().sprite = wh_bishop; break;
            case "wh_knight": this.GetComponent<SpriteRenderer>().sprite = wh_knight; break;
            case "wh_pawn": this.GetComponent<SpriteRenderer>().sprite = wh_pawn; break;
            case "wh_king": this.GetComponent<SpriteRenderer>().sprite = wh_king; break;
        }

        SetCoords();
    }
    public void SetCoords()
    {
        float x = xBoard.Value;
        float y = yBoard.Value;
        x *= 1.0f;
        y *= 1.0f;
        x += -3.5f;
        y += -3.5f;
        this.transform.position = new Vector3(x, y, -1.0f);
    }
    public int GetXBoard() { return xBoard.Value; }
    public int GetYBoard() { return yBoard.Value; }
    [ServerRpc(RequireOwnership = false)]
    public void SetXBoardServerRpc(int x) { xBoard.Value = x; }
    [ServerRpc(RequireOwnership = false)]
    public void SetYBoardServerRpc(int y) { yBoard.Value = y; }

    private void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().currentPlayer.Value.ToString() == player.Value)
        {
            Debug.Log(controller.GetComponent<Game>().GetCurrentPlayer() + " " + this.name + " " + xBoard.Value + " " + yBoard.Value);
            DestroyMovePlatesServerRpc();
            InitiateMovePlates();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyMovePlatesServerRpc()
    {
        GameObject[] plates = GameObject.FindGameObjectsWithTag("MovePlate");
        foreach (GameObject plate in plates)
        {
            plate.GetComponent<NetworkObject>().Despawn();
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "bl_queen":
            case "wh_queen":
                LineMove(1, 0);
                LineMove(0, 1);
                LineMove(-1, 0);
                LineMove(0, -1);
                LineMove(1, 1);
                LineMove(-1, 1);
                LineMove(1, -1);
                LineMove(-1, -1);
                break;
            case "bl_knight":
            case "wh_knight":
                LMove();
                break;
            case "bl_bishop":
            case "wh_bishop":
                LineMove(1, 1);
                LineMove(-1, 1);
                LineMove(1, -1);
                LineMove(-1, -1);
                break;
            case "bl_rook":
            case "wh_rook":
                LineMove(1, 0);
                LineMove(0, 1);
                LineMove(-1, 0);
                LineMove(0, -1);
                break;
            case "bl_king":
            case "wh_king":
                SurroundMove();
                break;
            case "bl_pawn":
                PawnMove(xBoard.Value, yBoard.Value - 1);
                break;
            case "wh_pawn":
                PawnMove(xBoard.Value, yBoard.Value + 1);
                break;
        }
    }

    public void LineMove(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();
        int x = xBoard.Value + xIncrement;
        int y = yBoard.Value + yIncrement;
        while (sc.PosOnBoard(x, y) && sc.GetPos(x, y) == null)
        {
            MovePlateSpawnServerRpc(x, y);
            x += xIncrement;
            y += yIncrement;
        }
        if (sc.PosOnBoard(x, y) && sc.GetPos(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawnServerRpc(x, y);
        }
    }

    public void LMove()
    {
        PointMovePlate(xBoard.Value + 1, yBoard.Value + 2);
        PointMovePlate(xBoard.Value + 1, yBoard.Value - 2);
        PointMovePlate(xBoard.Value - 1, yBoard.Value + 2);
        PointMovePlate(xBoard.Value - 1, yBoard.Value - 2);
        PointMovePlate(xBoard.Value + 2, yBoard.Value + 1);
        PointMovePlate(xBoard.Value + 2, yBoard.Value - 1);
        PointMovePlate(xBoard.Value - 2, yBoard.Value + 1);
        PointMovePlate(xBoard.Value - 2, yBoard.Value - 1);
    }

    public void SurroundMove()
    {
        PointMovePlate(xBoard.Value + 1, yBoard.Value);
        PointMovePlate(xBoard.Value - 1, yBoard.Value);
        PointMovePlate(xBoard.Value, yBoard.Value + 1);
        PointMovePlate(xBoard.Value, yBoard.Value - 1);
        PointMovePlate(xBoard.Value + 1, yBoard.Value + 1);
        PointMovePlate(xBoard.Value - 1, yBoard.Value + 1);
        PointMovePlate(xBoard.Value + 1, yBoard.Value - 1);
        PointMovePlate(xBoard.Value - 1, yBoard.Value - 1);

        // Castling attempt
        TryCastling();
    }
    public void TryCastling()
    {
        Game sc = controller.GetComponent<Game>();
        if (hasMoved || this.name != "wh_king" && this.name != "bl_king")
            return;

        int row = (player.Value == "White") ? 0 : 7;

        // King-side (short) castling
        if (CanCastle(row, 7, 5, 6))
        {
            MovePlateSpawnServerRpc(6, row); // King moves here
        }

        // Queen-side (long) castling
        if (CanCastle(row, 0, 1, 2, 3))
        {
            MovePlateSpawnServerRpc(2, row); // King moves here
        }
    }

    private bool CanCastle(int kingRow, int rookCol, params int[] emptyCols)
    {
        Game sc = controller.GetComponent<Game>();
        NetworkObject rook = sc.GetPos(rookCol, kingRow);
        if (rook == null) return false;

        Chessman cm = rook.GetComponent<Chessman>();
        if (cm == null || cm.name != (player.Value == "White" ? "wh_rook" : "bl_rook") || cm.hasMoved)
            return false;

        foreach (int col in emptyCols)
        {
            if (sc.GetPos(col, kingRow) != null)
                return false;
        }

        return true;
    }


    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PosOnBoard(x, y))
        {
            NetworkObject cp = sc.GetPos(x, y);
            if (cp == null)
            {
                MovePlateSpawnServerRpc(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawnServerRpc(x, y);
            }
        }
    }

    public void PawnMove(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        int direction = (player.Value == "White") ? 1 : -1;
        int startRow = (player.Value == "White") ? 1 : 6;

        // Single move forward
        if (sc.PosOnBoard(x, y) && sc.GetPos(x, y) == null)
        {
            if (IsClient)
            {
                MovePlateSpawnServerRpc(x, y);
            }

            // Double move forward on first move only
            if (yBoard.Value == startRow && sc.PosOnBoard(x, y + direction) && sc.GetPos(x, y + direction) == null)
            {
                if (IsClient)
                {
                    MovePlateSpawnServerRpc(x, y + direction);
                }
            }
        }

        // Captures
        Debug.Log(x + 1 + " " + y);
        Debug.Log(sc.PosOnBoard(x + 1, y));
        Debug.Log(x + " " + y);
        Debug.Log(sc.GetPos(x+1, y));
        if (sc.PosOnBoard(x + 1, y) && sc.GetPos(x + 1, y) != null &&
            sc.GetPos(x + 1, y).GetComponent<Chessman>().player != player)
        {
            if (IsClient)
            {
                MovePlateAttackSpawnServerRpc(x + 1, y);
            }
        }
        if (sc.PosOnBoard(x - 1, y) && sc.GetPos(x - 1, y) != null &&
            sc.GetPos(x - 1, y).GetComponent<Chessman>().player != player)
        {
            if (IsClient)
            {
                MovePlateAttackSpawnServerRpc(x - 1, y);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MovePlateSpawnServerRpc(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;
        x *= 1.0f;
        y *= 1.0f;
        x += -3.5f;
        y += -3.5f;
        GameObject mp = Instantiate(plate, new Vector3(x, y, -2.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject.GetComponent<NetworkObject>());
        mpScript.SetCoords(matrixX, matrixY);
        mp.GetComponent<NetworkObject>().Spawn();
    }
    [ServerRpc(RequireOwnership = false)]
    public void MovePlateAttackSpawnServerRpc(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;
        x *= 1.0f;
        y *= 1.0f;
        x += -3.5f;
        y += -3.5f;
        GameObject mp = Instantiate(plate, new Vector3(x, y, -2.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject.GetComponent<NetworkObject>());
        mpScript.SetCoords(matrixX, matrixY);
        mp.GetComponent<NetworkObject>().Spawn();
    }
}
