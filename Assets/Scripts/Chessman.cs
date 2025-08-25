using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Chessman : NetworkBehaviour
{
    public GameObject button;
    public GameObject controller;
    public GameObject plate;
    public GameObject effectplate;
    public Transform board;
    public float squareSize = 1.0f;
    public Piece template;
    public NetworkObject constant;
    public NetworkVariable<FixedString64Bytes> player =
    new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<FixedString64Bytes> piecename =
        new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<int> x =
        new NetworkVariable<int>();
    public NetworkVariable<int> y =
        new NetworkVariable<int>();
    public NetworkVariable<int> xBoard = new NetworkVariable<int>(-1);
    public NetworkVariable<int> yBoard = new NetworkVariable<int>(-1);
    public NetworkVariable<bool> hasMoved =
        new NetworkVariable<bool>();
    public NetworkVariable<bool> hasSpelled =
        new NetworkVariable<bool>(false);
    public Piece variant;
    public NetworkVariable<int> xmodifier = new NetworkVariable<int>();
    public NetworkVariable<int> ymodifier = new NetworkVariable<int>();
    public List<Ability> queue = new List<Ability>();
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
        controller = GameObject.FindGameObjectWithTag("GameController");
        SetXBoardServerRpc(x.Value);
        SetYBoardServerRpc(y.Value);
        this.name = piecename.Value.ToString();
        Activate();
        constant = this.GetComponent<NetworkObject>();
    }


    public void Activate()
    {

        switch (this.name)
        {
            case string s when s.StartsWith("bl_"):
                foreach (Piece sa in controller.GetComponent<Game>().draft.blackarmy)
                {
                    if (this.name.EndsWith(sa.suit))
                    {
                        Debug.Log(sa.name);
                        this.variant = sa.Clone();
                        this.variant.owner = this.GetComponent<Chessman>();
                        this.GetComponent<SpriteRenderer>().sprite = variant.Skin;
                    }
                }
                break;
            case string s when s.StartsWith("wh_"):
                foreach (Piece sa in controller.GetComponent<Game>().draft.whitearmy)
                {
                    if (this.name.EndsWith(sa.suit))
                    {
                        Debug.Log(sa.name); 
                        this.variant = sa.Clone();
                        this.variant.owner = this.GetComponent<Chessman>();
                        this.GetComponent<SpriteRenderer>().sprite = variant.Skin;
                    }
                }
                break;
        }
    }

    public void SetCoords()
    {
        ReSetCoordsClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    public void ReSetCoordsClientRpc()
    {
        float x = (xBoard.Value - 3.5f) * squareSize;
        float y = (yBoard.Value - 3.5f) * squareSize;
        this.transform.position = new Vector3(
            board.position.x + x,
            board.position.y + y,
            -1.0f
        );
    }
    public int GetXBoard() { return xBoard.Value; }
    public int GetYBoard() { return yBoard.Value; }
    [ServerRpc(RequireOwnership = false)]
    public void SetXBoardServerRpc(int x) { xBoard.Value = x; }
    [ServerRpc(RequireOwnership = false)]
    public void SetYBoardServerRpc(int y) { yBoard.Value = y; }

    private void OnMouseUp()
    {
        Debug.Log(variant.name);
        Debug.Log(controller.GetComponent<Game>().GetCurrentPlayer() + " " + this.name + " " + xBoard.Value + " " + yBoard.Value + " " + player.Value);
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().currentPlayer.Value.ToString() == player.Value && controller.GetComponent<Game>().currentPlayer.Value == controller.GetComponent<Game>().currentColor.pColor)
        {
            DestroyMovePlatesServerRpc();
            if (queue.Count > 0)
            {
                foreach (var item in queue) {
                    resolve(item.name);
                }
                queue.Clear();
                if (hasMoved.Value == true)
                {
                    sethasspelledServerRpc(true);
                }
            }
            else
            {
                sethasspelledServerRpc(true);
            }
            if (hasMoved.Value == false)
            {
                InitiateMovePlates();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void sethasspelledServerRpc(bool v)
    {
        hasSpelled.Value = v;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyMovePlatesServerRpc()
    {
        GameObject[] plates = GameObject.FindGameObjectsWithTag("MovePlate");
        foreach (GameObject plate in plates)
        {
            if (plate.GetComponent<NetworkObject>().IsSpawned)
            {
                plate.GetComponent<NetworkObject>().Despawn();
            }
        }
    }
    [ClientRpc(RequireOwnership = false)]
    public void killbuttClientRpc()
    {
        Debug.Log("killin butt on client");
        GameObject[] butts = GameObject.FindGameObjectsWithTag("skillbutt");
        Debug.Log(butts);
        foreach (GameObject butt in butts)
        {
            Destroy(butt);
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.variant.move)
        {
            case "AnyDirectionMove":
                LineMoveServerRpc(1, 0);
                LineMoveServerRpc(0, 1);
                LineMoveServerRpc(-1, 0);
                LineMoveServerRpc(0, -1);
                LineMoveServerRpc(1, 1);
                LineMoveServerRpc(-1, 1);
                LineMoveServerRpc(1, -1);
                LineMoveServerRpc(-1, -1);
                break;

            case "L-Move":
                LMoveServerRpc();
                break;

            case "DiagonalMove":
                LineMoveServerRpc(1, 1);
                LineMoveServerRpc(-1, 1);
                LineMoveServerRpc(1, -1);
                LineMoveServerRpc(-1, -1);
                break;

            case "LineMove":
                LineMoveServerRpc(1, 0);
                LineMoveServerRpc(0, 1);
                LineMoveServerRpc(-1, 0);
                LineMoveServerRpc(0, -1);
                break;

            case "OneStepAnyDirection":
                SurroundMoveServerRpc();
                break;

            case "ForwardOneStep":
                if (this.player.Value == "White")
                    PawnMoveServerRpc(xBoard.Value, yBoard.Value + 1);
                else
                    PawnMoveServerRpc(xBoard.Value, yBoard.Value - 1);
                break;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void LineMoveServerRpc(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();
        int x = xBoard.Value + xIncrement;
        int y = yBoard.Value + yIncrement;
        while (sc.PosOnBoard(x, y) && sc.GetPos(x, y) == null)
        {
            MovePlateSpawnServerRpc(x, y, 0);
            x += xIncrement;
            y += yIncrement;
        }
        if (sc.PosOnBoard(x, y) && sc.GetPos(x, y).GetComponent<Chessman>().player.Value != player.Value)
        {
            MovePlateAttackSpawnServerRpc(x, y, 0);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LMoveServerRpc()
    {
        PointMovePlateServerRpc(xBoard.Value + 1, yBoard.Value + 2);
        PointMovePlateServerRpc(xBoard.Value + 1, yBoard.Value - 2);
        PointMovePlateServerRpc(xBoard.Value - 1, yBoard.Value + 2);
        PointMovePlateServerRpc(xBoard.Value - 1, yBoard.Value - 2);
        PointMovePlateServerRpc(xBoard.Value + 2, yBoard.Value + 1);
        PointMovePlateServerRpc(xBoard.Value + 2, yBoard.Value - 1);
        PointMovePlateServerRpc(xBoard.Value - 2, yBoard.Value + 1);
        PointMovePlateServerRpc(xBoard.Value - 2, yBoard.Value - 1);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SurroundMoveServerRpc()
    {
        PointMovePlateServerRpc(xBoard.Value + 1, yBoard.Value);
        PointMovePlateServerRpc(xBoard.Value - 1, yBoard.Value);
        PointMovePlateServerRpc(xBoard.Value, yBoard.Value + 1);
        PointMovePlateServerRpc(xBoard.Value, yBoard.Value - 1);
        PointMovePlateServerRpc(xBoard.Value + 1, yBoard.Value + 1);
        PointMovePlateServerRpc(xBoard.Value - 1, yBoard.Value + 1);
        PointMovePlateServerRpc(xBoard.Value + 1, yBoard.Value - 1);
        PointMovePlateServerRpc(xBoard.Value - 1, yBoard.Value - 1);

        // Castling attempt
        TryCastling();
    }
    public void TryCastling()
    {
        Game sc = controller.GetComponent<Game>();
        if (hasMoved.Value || this.name != "wh_king" && this.name != "bl_king")
            return;

        int row = (player.Value == "White") ? 0 : 7;

        // King-side (short) castling
        if (CanCastle(row, 7, 5, 6))
        {
            MovePlateSpawnServerRpc(6, row, 0); // King moves here
        }

        // Queen-side (long) castling
        if (CanCastle(row, 0, 1, 2, 3))
        {
            MovePlateSpawnServerRpc(2, row, 0); // King moves here
        }
    }

    private bool CanCastle(int kingRow, int rookCol, params int[] emptyCols)
    {
        Game sc = controller.GetComponent<Game>();
        NetworkObject rook = sc.GetPos(rookCol, kingRow);
        if (rook == null) return false;

        Chessman cm = rook.GetComponent<Chessman>();
        if (cm == null || cm.name != (player.Value == "White" ? "wh_rook" : "bl_rook") || cm.hasMoved.Value)
            return false;

        foreach (int col in emptyCols)
        {
            if (sc.GetPos(col, kingRow) != null)
                return false;
        }

        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PointMovePlateServerRpc(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PosOnBoard(x, y))
        {
            NetworkObject cp = sc.GetPos(x, y);
            if (cp == null)
            {
                MovePlateSpawnServerRpc(x, y, 0);
            }
            else if (cp.GetComponent<Chessman>().player.Value != player.Value)
            {
                MovePlateAttackSpawnServerRpc(x, y, 0);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PawnMoveServerRpc(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        int direction = (player.Value == "White") ? 1 : -1;
        int startRow = (player.Value == "White") ? 1 : 6;

        // Single move forward
        sc.GetPosServerRpc(xBoard.Value, yBoard.Value);
        if (sc.PosOnBoard(x, y) && sc.GetPos(x, y) == null)
        {
            if (IsClient)
            {
                MovePlateSpawnServerRpc(x, y, 0);
            }

            // Double move forward on first move only
            if (yBoard.Value == startRow && sc.PosOnBoard(x, y + direction) && sc.GetPos(x, y + direction) == null)
            {
                if (IsClient)
                {
                    MovePlateSpawnServerRpc(x, y + direction, 0);
                }
            }
        }

        // Captures
        if (sc.PosOnBoard(x + 1, y) && sc.GetPos(x + 1, y) != null &&
            sc.GetPos(x + 1, y).GetComponent<Chessman>().player.Value != player.Value)
        {   
            if (IsClient)
            {
                MovePlateAttackSpawnServerRpc(x + 1, y, 0);
            }
        }
        if (sc.PosOnBoard(x - 1, y) && sc.GetPos(x - 1, y) != null &&
            sc.GetPos(x - 1, y).GetComponent<Chessman>().player.Value != player.Value)
        {
            if (IsClient)
            {
                MovePlateAttackSpawnServerRpc(x - 1, y, 0);
            }
        }
    }

    
    [ServerRpc(RequireOwnership = false)]
    public void MovePlateSpawnServerRpc(int matrixX, int matrixY, int type)
    {
        float x = matrixX;
        float y = matrixY;
        x *= 1.0f;
        y *= 1.0f;
        x += -3.5f;
        y += -3.5f;
        GameObject mp = Instantiate(plate, new Vector3(x, y, -2.0f), Quaternion.identity);
        MovePlate mpScript = null;
        switch (type)
        {
            case 0:
                mpScript = mp.GetComponent<MovePlate>();
                break;
            case 1:
                mpScript = mp.GetComponent<explosionplate>();
                break;
            case 2:
                mpScript = mp.GetComponent<MovePlate>();
                mpScript.esploding.Value = true;
                break;
        }
        mpScript.SetReference(gameObject.GetComponent<NetworkObject>());
        mpScript.SetCoords(matrixX, matrixY);
        mp.GetComponent<NetworkObject>().Spawn();
        if (player.Value == "Black")
        {
            mpScript.SetColorClientRpc(new Color(255f, 0f, 88f, 1f));
        }
        else if (player.Value == "White")
        {
            mpScript.SetColorClientRpc(new Color(0f, 176f, 255f, 1f));
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void MovePlateAttackSpawnServerRpc(int matrixX, int matrixY, int type)
    {
        float x = matrixX;
        float y = matrixY;
        x *= 1.0f;
        y *= 1.0f;
        x += -3.5f;
        y += -3.5f;
        GameObject mp = Instantiate(plate, new Vector3(x, y, -2.0f), Quaternion.identity);
        MovePlate mpScript = null;
        switch (type)
        {
            case 0:
                mpScript = mp.GetComponent<MovePlate>();
                break;
            case 2:
                mpScript.esploding.Value = true;
                break;
        }
        mpScript.attack.Value = true;
        mpScript.SetReference(gameObject.GetComponent<NetworkObject>());
        mpScript.SetCoords(matrixX, matrixY);
        mp.GetComponent<NetworkObject>().Spawn();
        mpScript.SetColorClientRpc(new Color(1f, 0f, 0f, 1f));
    }
    [ServerRpc(RequireOwnership = false)]
    public void DmgslashmarkerServerRpc(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;
        x *= 1.0f;
        y *= 1.0f;
        x += -3.5f;
        y += -3.5f;
        GameObject mp = Instantiate(effectplate, new Vector3(x, y, -2.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<explosionplate>();
        mpScript.SetReference(gameObject.GetComponent<NetworkObject>());
        mpScript.SetCoords(matrixX, matrixY);
        mp.GetComponent<NetworkObject>().Spawn();
        if (player.Value == "Black")
        {
            mpScript.SetColorClientRpc(new Color(255f, 0f, 88f, 1f));
        }
        else if (player.Value == "White")
        {
            mpScript.SetColorClientRpc(new Color(0f, 176f, 255f, 1f));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EffectSpawnServerRpc(int type, int matrixX, int matrixY)
    {
        Debug.Log(type);
        MovePlateAttackSpawnServerRpc(matrixX, matrixY, type);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ActionsServerRpc()
    {
        if(variant != null && controller.GetComponent<Game>().currentPlayer.Value.ToString() == player.Value.ToString())
        {
            variant.Passive();
        }
    }
    public void runcheck()
    {
        Debug.Log("running check" + hasMoved.Value);
    }

    public void SetButton()
    {
        if (variant != null)
        {
            foreach (Ability ability in variant.kit)
            {
                if (!hasMoved.Value)
                {
                    if (ability.type == "premove")
                    {
                        Debug.Log(ability);
                        Transform grandchild = transform.Find("Canvas/Image");
                        GameObject skillbutt = Instantiate(button, grandchild);
                        skillbutt.name = ability.name;
                        skillbutt.GetComponentInChildren<TMP_Text>().text = ability.name;
                        Vector3 pos = skillbutt.transform.position;
                        pos.z = -5;
                        skillbutt.transform.position = pos;
                        skillbutt.GetComponent<Button>().onClick.AddListener(() => resolve(ability.name));
                    }
                }
                else if (hasMoved.Value)
                {
                    if (ability.type == "postmove")
                    {
                        Debug.Log(ability);
                        Transform grandchild = transform.Find("Canvas/Image");
                        GameObject skillbutt = Instantiate(button, grandchild);
                        skillbutt.name = ability.name;
                        Vector3 pos = skillbutt.transform.position;
                        pos.z = -5;
                        skillbutt.transform.position = pos;
                        skillbutt.GetComponentInChildren<TMP_Text>().text = ability.name;
                        skillbutt.GetComponent<Button>().onClick.AddListener(() => resolve(ability.name));
                    }
                }
            }
        }
    }

    public void resolve(string name)
    {
        Debug.Log("resolving " +  name);
        Game sc = controller.GetComponent<Game>();
        foreach (Ability ability in variant.kit)
        {
            if(ability.name == name)
            {
                Debug.Log(ability);
                SkillServerRpc(name, ability.type);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SkillServerRpc(string name, string type)
    {
        Debug.Log($"{variant.name}/{name}");
        switch (type)
        {
            case "premove":
                variant.PreMove(name);
                break;
            case "postmove":
                variant.PostMove(name);
                break;
        }
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void changeturnServerRpc()
    {
        controller.GetComponent<Game>().currentPlayer.Value = (controller.GetComponent<Game>().currentPlayer.Value.ToString() == "White") ? "Black" : "White";
        hasMoved.Value = false;
        hasSpelled.Value = false;
        controller.GetComponent<Game>().phase.Value = "premove";
    }
    void Update()
    {
        if (hasMoved.Value)
        {
            controller.GetComponent<Game>().phase.Value = "postmove";
        }
        SetCoords();
        if (hasMoved.Value) { 

        }
        ActionsServerRpc();
        if (hasMoved.Value && hasSpelled.Value)
        {
            changeturnServerRpc();
        }
    }
}
