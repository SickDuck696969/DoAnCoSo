using UnityEngine;
using UnityEngine.WSA;

public class Chessmanoff : MonoBehaviour
{
    public GameObject controller;
    public GameObject plate;
    private int xBoard = -1;
    private int yBoard = -1;
    public string player;
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
    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        SetCoords();
        switch (this.name)
        {
            case "bl_queen": this.GetComponent<SpriteRenderer>().sprite = bl_queen; player = "Black"; break;
            case "bl_rook": this.GetComponent<SpriteRenderer>().sprite = bl_rook; player = "Black"; break;
            case "bl_bishop": this.GetComponent<SpriteRenderer>().sprite = bl_bishop; player = "Black"; break;
            case "bl_knight": this.GetComponent<SpriteRenderer>().sprite = bl_knight; player = "Black"; break;
            case "bl_pawn": this.GetComponent<SpriteRenderer>().sprite = bl_pawn; player = "Black"; break;
            case "bl_king": this.GetComponent<SpriteRenderer>().sprite = bl_king; player = "Black"; break;
            case "wh_queen": this.GetComponent<SpriteRenderer>().sprite = wh_queen; player = "White"; break;
            case "wh_rook": this.GetComponent<SpriteRenderer>().sprite = wh_rook; player = "White"; break;
            case "wh_bishop": this.GetComponent<SpriteRenderer>().sprite = wh_bishop; player = "White"; break;
            case "wh_knight": this.GetComponent<SpriteRenderer>().sprite = wh_knight; player = "White"; break;
            case "wh_pawn": this.GetComponent<SpriteRenderer>().sprite = wh_pawn; player = "White"; break;
            case "wh_king": this.GetComponent<SpriteRenderer>().sprite = wh_king; player = "White"; break;
        }
    }
    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;
        x *= 1.0f;
        y *= 1.0f;
        x += -3.5f;
        y += -3.5f;
        this.transform.position = new Vector3(x, y, -1.0f);
    }
    public int GetXBoard() { return xBoard; }
    public int GetYBoard() { return yBoard; }
    public void SetXBoard(int x) { xBoard = x; }
    public void SetYBoard(int y) { yBoard = y; }

    private void OnMouseUp()
    {
        if (!controller.GetComponent<Gameoff>().IsGameOver() && controller.GetComponent<Gameoff>().GetCurrentPlayer() == player)
        {
            DestroyMovePlates();
            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        GameObject[] plates = GameObject.FindGameObjectsWithTag("MovePlate");
        foreach (GameObject plate in plates)
        {
            Destroy(plate);
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
                PawnMove(xBoard, yBoard - 1);
                break;
            case "wh_pawn":
                PawnMove(xBoard, yBoard + 1);
                break;
        }
    }

    public void LineMove(int xIncrement, int yIncrement)
    {
        Gameoff sc = controller.GetComponent<Gameoff>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        while (sc.PosOnBoard(x, y) && sc.GetPos(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }
        if (sc.PosOnBoard(x, y) && sc.GetPos(x, y).GetComponent<Chessmanoff>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    public void LMove()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMove()
    {
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);

        // Castling attempt
        TryCastling();
    }
    public void TryCastling()
    {
        Gameoff sc = controller.GetComponent<Gameoff>();
        if (hasMoved || this.name != "wh_king" && this.name != "bl_king")
            return;

        int row = (player == "White") ? 0 : 7;

        // King-side (short) castling
        if (CanCastle(row, 7, 5, 6))
        {
            MovePlateSpawn(6, row); // King moves here
        }

        // Queen-side (long) castling
        if (CanCastle(row, 0, 1, 2, 3))
        {
            MovePlateSpawn(2, row); // King moves here
        }
    }

    private bool CanCastle(int kingRow, int rookCol, params int[] emptyCols)
    {
        Gameoff sc = controller.GetComponent<Gameoff>();
        GameObject rook = sc.GetPos(rookCol, kingRow);
        if (rook == null) return false;

        Chessmanoff cm = rook.GetComponent<Chessmanoff>();
        if (cm == null || cm.name != (player == "White" ? "wh_rook" : "bl_rook") || cm.hasMoved)
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
        Gameoff sc = controller.GetComponent<Gameoff>();
        if (sc.PosOnBoard(x, y))
        {
            GameObject cp = sc.GetPos(x, y);
            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessmanoff>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void PawnMove(int x, int y)
    {
        Gameoff sc = controller.GetComponent<Gameoff>();

        int direction = (player == "White") ? 1 : -1;
        int startRow = (player == "White") ? 1 : 6;

        // Single move forward
        if (sc.PosOnBoard(x, y) && sc.GetPos(x, y) == null)
        {
            MovePlateSpawn(x, y);

            // Double move forward on first move only
            if (yBoard == startRow && sc.PosOnBoard(x, y + direction) && sc.GetPos(x, y + direction) == null)
            {
                MovePlateSpawn(x, y + direction);
            }
        }

        // Captures
        if (sc.PosOnBoard(x + 1, y) && sc.GetPos(x + 1, y) != null &&
            sc.GetPos(x + 1, y).GetComponent<Chessmanoff>().player != player)
        {
            MovePlateAttackSpawn(x + 1, y);
        }
        if (sc.PosOnBoard(x - 1, y) && sc.GetPos(x - 1, y) != null &&
            sc.GetPos(x - 1, y).GetComponent<Chessmanoff>().player != player)
        {
            MovePlateAttackSpawn(x - 1, y);
        }
    }


    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;
        x *= 1.0f;
        y *= 1.0f;
        x += -3.5f;
        y += -3.5f;
        GameObject mp = Instantiate(plate, new Vector3(x, y, -2.0f), Quaternion.identity);
        MovePlateoff mpScript = mp.GetComponent<MovePlateoff>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;
        x *= 1.0f;
        y *= 1.0f;
        x += -3.5f;
        y += -3.5f;
        GameObject mp = Instantiate(plate, new Vector3(x, y, -2.0f), Quaternion.identity);
        MovePlateoff mpScript = mp.GetComponent<MovePlateoff>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
}
