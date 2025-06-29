using UnityEngine;

public class MovePlate : MonoBehaviour
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

        Chessman cmRef = reference.GetComponent<Chessman>();
        int startX = cmRef.GetXBoard();
        int startY = cmRef.GetYBoard();

        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPos(matrixX, matrixY);
            if (cp.name == "bl_king" || cp.name == "wh_king")
            {
                controller.GetComponent<Game>().Winner(cmRef.player);
            }
            Destroy(cp);
        }

        // Castling logic BEFORE moving the king
        if ((cmRef.name == "wh_king" || cmRef.name == "bl_king") && Mathf.Abs(matrixX - startX) == 2)
        {
            // King-side
            if (matrixX == 6)
            {
                GameObject rook = controller.GetComponent<Game>().GetPos(7, matrixY);
                controller.GetComponent<Game>().SetPosEmpty(7, matrixY);
                rook.GetComponent<Chessman>().SetXBoard(5);
                rook.GetComponent<Chessman>().SetCoords();
                controller.GetComponent<Game>().SetPos(rook);
                rook.GetComponent<Chessman>().hasMoved = true;
            }
            // Queen-side
            else if (matrixX == 2)
            {
                GameObject rook = controller.GetComponent<Game>().GetPos(0, matrixY);
                controller.GetComponent<Game>().SetPosEmpty(0, matrixY);
                rook.GetComponent<Chessman>().SetXBoard(3);
                rook.GetComponent<Chessman>().SetCoords();
                controller.GetComponent<Game>().SetPos(rook);
                rook.GetComponent<Chessman>().hasMoved = true;
            }
        }

        // Clear old position
        controller.GetComponent<Game>().SetPosEmpty(startX, startY);

        // Move the piece (king or regular move)
        cmRef.SetXBoard(matrixX);
        cmRef.SetYBoard(matrixY);
        cmRef.SetCoords();
        controller.GetComponent<Game>().SetPos(reference);
        cmRef.hasMoved = true;

        controller.GetComponent<Game>().NextTurn();
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
