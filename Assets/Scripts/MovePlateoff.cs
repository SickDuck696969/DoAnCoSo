using UnityEngine;

public class MovePlateoff : MonoBehaviour
{
    public GameObject controller;
    GameObject reference = null;
    int matrixX;
    int matrixY;
    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        Chessmanoff cmRef = reference.GetComponent<Chessmanoff>();
        int startX = cmRef.GetXBoard();
        int startY = cmRef.GetYBoard();

        if (attack)
        {
            GameObject cp = controller.GetComponent<Gameoff>().GetPos(matrixX, matrixY);
            if (cp.name == "bl_king" || cp.name == "wh_king")
            {
                controller.GetComponent<Gameoff>().Winner(cmRef.player);
            }
            Destroy(cp);
        }

        // Castling logic BEFORE moving the king
        if ((cmRef.name == "wh_king" || cmRef.name == "bl_king") && Mathf.Abs(matrixX - startX) == 2)
        {
            // King-side
            if (matrixX == 6)
            {
                GameObject rook = controller.GetComponent<Gameoff>().GetPos(7, matrixY);
                controller.GetComponent<Gameoff>().SetPosEmpty(7, matrixY);
                rook.GetComponent<Chessmanoff>().SetXBoard(5);
                rook.GetComponent<Chessmanoff>().SetCoords();
                controller.GetComponent<Gameoff>().SetPos(rook);
                rook.GetComponent<Chessmanoff>().hasMoved = true;
            }
            // Queen-side
            else if (matrixX == 2)
            {
                GameObject rook = controller.GetComponent<Gameoff>().GetPos(0, matrixY);
                controller.GetComponent<Gameoff>().SetPosEmpty(0, matrixY);
                rook.GetComponent<Chessmanoff>().SetXBoard(3);
                rook.GetComponent<Chessmanoff>().SetCoords();
                controller.GetComponent<Gameoff>().SetPos(rook);
                rook.GetComponent<Chessmanoff>().hasMoved = true;
            }
        }

        // Clear old position
        controller.GetComponent<Gameoff>().SetPosEmpty(startX, startY);

        // Move the piece (king or regular move)
        cmRef.SetXBoard(matrixX);
        cmRef.SetYBoard(matrixY);
        cmRef.SetCoords();
        controller.GetComponent<Gameoff>().SetPos(reference);
        cmRef.hasMoved = true;

        controller.GetComponent<Gameoff>().NextTurn();
        cmRef.DestroyMovePlates();
    }



    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}
