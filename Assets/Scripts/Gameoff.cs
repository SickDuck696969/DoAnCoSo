using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameoff : MonoBehaviour
{
    public GameObject piece;
    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];
    private string currentPlayer = "White";
    private bool gameOver = false;

    void Start()
    {
        playerWhite = new GameObject[] {
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

        playerBlack = new GameObject[] {
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
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(piece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessmanoff cm = obj.GetComponent<Chessmanoff>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        cm.hasMoved = false;
        return obj;
    }

    public void SetPos(GameObject obj)
    {
        Chessmanoff cm = obj.GetComponent<Chessmanoff>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPosEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPos(int x, int y)
    {
        return positions[x, y];
    }

    public bool PosOnBoard(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1));
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        currentPlayer = (currentPlayer == "White") ? "Black" : "White";
    }

    public void Update()
    {
        if (gameOver && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
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
