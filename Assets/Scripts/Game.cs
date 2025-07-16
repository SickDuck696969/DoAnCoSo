using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : NetworkBehaviour
{
    public GameObject piece;
    public NetworkObject[,] positions = new NetworkObject[8, 8];
    private NetworkObject[] playerBlack = new NetworkObject[16];
    private NetworkObject[] playerWhite = new NetworkObject[16];
    public NetworkVariable<FixedString64Bytes> currentPlayer =
    new NetworkVariable<FixedString64Bytes>("White", NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);
    private bool gameOver = false;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            currentPlayer.OnValueChanged += (oldValue, newValue) =>
            {
                Debug.Log($"Current player changed from {oldValue} to {newValue}");
            };
        }

        if (IsServer)
        {
            StartCoroutine(DelayedInit());
        }
    }

    private IEnumerator DelayedInit()
    {
        yield return null;

        init();
    }

    void init()
    {
        currentPlayer.Value = "White";
        playerWhite = new NetworkObject[] {
            Create("wh_rook", 0, 0),
            Create("wh_knight", 1, 0),
            Create("wh_bishop", 2, 0),
            Create("wh_queen", 3, 0),
            Create("wh_king", 4, 0),
            Create("wh_bishop", 5, 0),
            Create("wh_knight", 6, 0),
            Create("wh_rook", 7, 0),
            Create("wh_pawn", 0, 1),
            Create("wh_pawn", 1, 1),
            Create("wh_pawn", 2, 1),
            Create("wh_pawn", 3, 1),
            Create("wh_pawn", 4, 1),
            Create("wh_pawn", 5, 1),
            Create("wh_pawn", 6, 1),
            Create("wh_pawn", 7, 1)
        };

        playerBlack = new NetworkObject[] {
            Create("bl_rook", 0, 7),
            Create("bl_knight", 1, 7),
            Create("bl_bishop", 2, 7),
            Create("bl_queen", 3, 7),
            Create("bl_king", 4, 7),
            Create("bl_bishop", 5, 7),
            Create("bl_knight", 6, 7),
            Create("bl_rook", 7, 7),
            Create("bl_pawn", 0, 6),
            Create("bl_pawn", 1, 6),
            Create("bl_pawn", 2, 6),
            Create("bl_pawn", 3, 6),
            Create("bl_pawn", 4, 6),
            Create("bl_pawn", 5, 6),
            Create("bl_pawn", 6, 6),
            Create("bl_pawn", 7, 6)
        };

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPos(playerBlack[i]);
            SetPos(playerWhite[i]);
        }

        GetDateFromServer();
    }

    IEnumerator GetDateFromServer()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/testdating/getthedate.php");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to contact server: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            Debug.Log("Server time is: " + json);
        }
    }

    [ServerRpc]
    public void RequestSpawnPlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        GameObject obj = Instantiate(piece, new Vector3(0, 0, -1), Quaternion.identity);
        obj.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }

    public NetworkObject Create(string name, int x, int y)
    {
        if (!IsServer) return null;


        GameObject obj = Instantiate(piece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.x.Value = x;
        cm.y.Value = y;
        cm.piecename.Value = name;
        obj.GetComponent<NetworkObject>().Spawn();
        switch (cm.name)
        {
            case string s when s.StartsWith("bl_"):
                cm.player.Value = "Black";
                break;
            case string s when s.StartsWith("wh_"):
                cm.player.Value = "White";
                break;
        }
        cm.hasMoved = false;
        return obj.GetComponent<NetworkObject>();
    }

    public void SetPos(NetworkObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPosEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public NetworkObject GetPos(int x, int y)
    {
        return positions[x, y];
    }

    public bool PosOnBoard(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1));
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer.Value.ToString();
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void Update()
    {
        if (!IsOwner) return;
        if (gameOver && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            init();
        }
    }


    public void Winner(string playerWinner)
    {
        gameOver = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " wins!";
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }
}
