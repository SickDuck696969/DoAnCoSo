using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Gameoff : MonoBehaviour
{
    public bool AI = false;
    public StockfishController stockfishController;
    public GameObject piece;
    public GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];
    public GameObject ResultPanel;
    public TextMeshProUGUI ResultText;
    private string currentPlayer = "White";
    public string yourcolor;
    private bool gameOver = false;
    public string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public string alphabet = "abcdefgh";

    void Start()
    {
        if (AI) {
            yourcolor = "White";
            if(yourcolor == "White")
            {
                fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            } else
            {
                fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
            }
            if(currentPlayer != yourcolor)
            {
                generatefen();
            }
        }
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
        Debug.Log(cm.xBoard + " " + cm.yBoard);
        positions[cm.xBoard, cm.yBoard] = obj;
        Debug.Log(GetPos(cm.xBoard, cm.yBoard));
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

    public void generatefen()
    {
        fen = "";
        int height = positions.GetLength(1);
        int width = positions.GetLength(0);

        for (int y = height - 1; y >= 0; y--)
        {
            int emptyCount = 0;

            for (int x = 0; x < width; x++)
            {
                GameObject piece = GetPos(x, y);
                string squareString = "";

                if (piece != null)
                {
                    if (piece.name.EndsWith("pawn")) squareString = "p";
                    else if (piece.name.EndsWith("rook")) squareString = "r";
                    else if (piece.name.EndsWith("knight")) squareString = "n";
                    else if (piece.name.EndsWith("bishop")) squareString = "b";
                    else if (piece.name.EndsWith("queen")) squareString = "q";
                    else if (piece.name.EndsWith("king")) squareString = "k";

                    if (piece.name.StartsWith("wh")) squareString = squareString.ToUpper();

                    if (emptyCount > 0)
                    {
                        fen += emptyCount.ToString();
                        emptyCount = 0;
                    }

                    fen += squareString;
                }
                else
                {
                    emptyCount++;
                }
            }

            if (emptyCount > 0)
            {
                fen += emptyCount.ToString();
            }

            if (y != 0)
            {
                fen += "/";
            }
        }

        if (currentPlayer == "White") fen += " w";
        else {fen += " b"; }


        fen += " KQkq - 0 1";

        StartCoroutine(stockfishController.GetBestMove(fen, move =>
        {
            Debug.Log(fen);
            Debug.Log(move);
            string startpoint = move.Substring(0, 2);
            string endpoint = move.Substring(move.Length - 2);
            GameObject en = GetPos(alphabet.IndexOf(endpoint.Substring(0, 1)), int.Parse(endpoint.Substring(endpoint.Length - 1)) - 1);
            GameObject st = GetPos(alphabet.IndexOf(startpoint.Substring(0, 1)), int.Parse(startpoint.Substring(startpoint.Length - 1)) - 1);
            Chessmanoff cm = st.GetComponent<Chessmanoff>();
            Debug.Log(en);
            Debug.Log(st);
            if (en != null)
            {
                if (en.name == "bl_king" || en.name == "wh_king")
                {
                    Winner(cm.player);
                }
                Destroy(en);
            }
            SetPosEmpty(cm.xBoard, cm.yBoard);
            cm.xBoard = alphabet.IndexOf(endpoint.Substring(0, 1));
            cm.yBoard = int.Parse(endpoint.Substring(endpoint.Length - 1)) - 1;
            Debug.Log(cm.xBoard + " " + cm.yBoard);
            float x = cm.xBoard;
            float y = cm.yBoard;
            x *= 1.0f;
            y *= 1.0f;
            x += -3.5f;
            y += -3.5f;
            cm.transform.position = new Vector3(x, y, -1.0f);
            SetPos(st);
            cm.hasMoved = true;

            Debug.Log(GetPos(alphabet.IndexOf(endpoint.Substring(0, 1)), int.Parse(endpoint.Substring(endpoint.Length - 1)) - 1));
            NextTurn();
        }));
    }

    public void NextTurn()
    {
        currentPlayer = (currentPlayer == "White") ? "Black" : "White";
        if (AI)
        {
            if(currentPlayer != yourcolor)
            {
                generatefen();
            }
        }else
        {
            generatefen();
        }
    }



    public void Winner(string currentPlayer)
    {
        gameOver = true;
        ResultPanel.SetActive(true);
        ResultText.text = currentPlayer + " wins!";
    }
}
