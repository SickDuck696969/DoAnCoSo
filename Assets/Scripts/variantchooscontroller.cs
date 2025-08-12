using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class variantchooscontroller : MonoBehaviour
{
    public Player l;
    public GameObject bg;
    public Button asd;

    public GameObject playerbox;
    public GameObject piecebox;

    public TMP_Text a;

    public GameObject row1col1;
    public GameObject row1col2;
    public GameObject row1col3;

    public GameObject row2col1;
    public GameObject row2col2;
    public GameObject row2col3;

    public GameObject row3col1;
    public GameObject row3col2;
    public GameObject row3col3;

    public GameObject pawnButton;
    public GameObject knightButton;
    public GameObject bishopButton;
    public GameObject rookButton;
    public GameObject queenButton;
    public GameObject kingButton;

    public Army draft;

    public GameObject variantprefab;

    public List<Piece> Pawnlist = new List<Piece>();
    public List<Piece> Knightlist = new List<Piece>();
    public List<Piece> Bishoplist = new List<Piece>();
    public List<Piece> Rooklist = new List<Piece>();
    public List<Piece> Queenlist = new List<Piece>();
    public List<Piece> Kinglist = new List<Piece>();


    GameObject[] row1;
    GameObject[] row2;
    GameObject[] row3;

    public List<Piece> army = new List<Piece>();

    public void init()
    {
        army.Clear();

        Pawn pawn = new Pawn();
        pawn.Skin = Resources.Load<Sprite>("chess-pawn-"+l.pColor);
        army.Add(pawn);

        Knight knight = new Knight();
        knight.Skin = Resources.Load<Sprite>("chess-knight-" + l.pColor);
        army.Add(knight);

        Bishop bishop = new Bishop();
        bishop.Skin = Resources.Load<Sprite>("chess-bishop-" + l.pColor);
        army.Add(bishop);

        Rook rook = new Rook();
        rook.Skin = Resources.Load<Sprite>("chess-rook-" + l.pColor);
        army.Add(rook);

        Queen queen = new Queen();
        queen.Skin = Resources.Load<Sprite>("chess-queen-" + l.pColor);
        army.Add(queen);

        King king = new King();
        king.Skin = Resources.Load<Sprite>("chess-king-" + l.pColor);
        army.Add(king);

        MaskedKnight maskedknight = new MaskedKnight();
        Debug.Log(maskedknight.suit);
        maskedknight.Skin = Resources.Load<Sprite>("chess-masked-knight-" + l.pColor);
        army.Add(maskedknight);
    }

    void Start()
    {
        init();
        row1 = new GameObject[] { row1col1, row1col2, row1col3 };
        row2 = new GameObject[] { row2col1, row2col2, row2col3 };
        row3 = new GameObject[] { row3col1, row1col3, row3col3 };
        // Add listeners for each button's OnClick event
        pawnButton.GetComponent<Button>().onClick.AddListener(OnPawnClicked);
        knightButton.GetComponent<Button>().onClick.AddListener(OnKnightClicked);
        bishopButton.GetComponent<Button>().onClick.AddListener(OnBishopClicked);
        rookButton.GetComponent<Button>().onClick.AddListener(OnRookClicked);
        queenButton.GetComponent<Button>().onClick.AddListener(OnQueenClicked);
        kingButton.GetComponent<Button>().onClick.AddListener(OnKingClicked);

        foreach (Piece cp in army)
        {
            if (cp.suit == "pawn")
            {
                Pawnlist.Add(cp);
            }
            else if (cp.suit == "knight")
            {
                Debug.Log("loged");
                Knightlist.Add(cp);
            }
            else if (cp.suit == "bishop")
            {
                Bishoplist.Add(cp);
            }
            else if (cp.suit == "rook")
            {
                Rooklist.Add(cp);
            }
            else if (cp.suit == "queen")
            {
                Queenlist.Add(cp);
            }
            else if (cp.suit == "king")
            {
                Kinglist.Add(cp);
            }
            Delay(2f);
            pawnButton.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("chess-pawn-" + l.pColor);
            knightButton.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("chess-knight-" + l.pColor);
            bishopButton.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("chess-bishop-" + l.pColor);
            rookButton.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("chess-rook-" + l.pColor);
            queenButton.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("chess-queen-" + l.pColor);
            kingButton.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("chess-king-" + l.pColor);

            asd.onClick.AddListener(moveone);
        }
}
    IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    public void moveone()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    // Button click handler functions
    void OnPawnClicked()
    {
        a.text = "PAWN";
        GameObject[] butts = GameObject.FindGameObjectsWithTag("skillbutt");
        Debug.Log(butts);
        foreach (GameObject butt in butts)
        {
            Destroy(butt);
        }
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            row1col1.GetComponent<variantchoso>().variant = new Pawn();
            row1col2.GetComponent<variantchoso>().variant = new Pawn();
            row1col3.GetComponent<variantchoso>().variant = new Pawn();

            row2col1.GetComponent<variantchoso>().variant = new Pawn();
            row2col2.GetComponent<variantchoso>().variant = new Pawn();
            row2col3.GetComponent<variantchoso>().variant = new Pawn();

            row3col1.GetComponent<variantchoso>().variant = new Pawn();
            row3col2.GetComponent<variantchoso>().variant = new Pawn();
            row3col3.GetComponent<variantchoso>().variant = new Pawn();


            foreach (Piece cp in Pawnlist)
            {
                if(cp != null)
                {
                    int a = Pawnlist.IndexOf(cp);
                    if(a != -1)
                    {
                        if (a <= 3)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }else if (a > 4 && a <= 7)
                        {
                            row2[a].GetComponent<variantchoso>().variant = cp;
                        }else if (a > 6 && a <= 9)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnKnightClicked()
    {
        if(l.pColor == "White") a.color = Color.white;
        a.text = "KNIGHT";
        GameObject[] butts = GameObject.FindGameObjectsWithTag("skillbutt");
        Debug.Log(butts);
        foreach (GameObject butt in butts)
        {
            Destroy(butt);
        }
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            row1col1.GetComponent<variantchoso>().variant = new Knight();
            row1col2.GetComponent<variantchoso>().variant = new Knight();
            row1col3.GetComponent<variantchoso>().variant = new Knight();

            row2col1.GetComponent<variantchoso>().variant = new Knight();
            row2col2.GetComponent<variantchoso>().variant = new Knight();
            row2col3.GetComponent<variantchoso>().variant = new Knight();

            row3col1.GetComponent<variantchoso>().variant = new Knight();
            row3col2.GetComponent<variantchoso>().variant = new Knight();
            row3col3.GetComponent<variantchoso>().variant = new Knight();


            foreach (Piece cp in Knightlist)
            {
                if (cp != null)
                {
                    int a = Knightlist.IndexOf(cp);
                    Debug.Log(a);
                    if (a != -1)
                    {
                        if (a <= 3)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 4 && a <= 7)
                        {
                            row2[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 6 && a <= 9)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnBishopClicked()
    {
        if (l.pColor == "White") a.color = Color.white;
        a.text = "BISHOP";
        GameObject[] butts = GameObject.FindGameObjectsWithTag("skillbutt");
        Debug.Log(butts);
        foreach (GameObject butt in butts)
        {
            Destroy(butt);
        }
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            row1col1.GetComponent<variantchoso>().variant = new Bishop();
            row1col2.GetComponent<variantchoso>().variant = new Bishop();
            row1col3.GetComponent<variantchoso>().variant = new Bishop();

            row2col1.GetComponent<variantchoso>().variant = new Bishop();
            row2col2.GetComponent<variantchoso>().variant = new Bishop();
            row2col3.GetComponent<variantchoso>().variant = new Bishop();

            row3col1.GetComponent<variantchoso>().variant = new Bishop();
            row3col2.GetComponent<variantchoso>().variant = new Bishop();
            row3col3.GetComponent<variantchoso>().variant = new Bishop();

            foreach (Piece cp in Bishoplist)
            {
                if (cp != null)
                {
                    int a = Bishoplist.IndexOf(cp);
                    if (a != -1)
                    {
                        if (a <= 3)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 4 && a <= 7)
                        {
                            row2[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 6 && a <= 9)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnRookClicked()
    {
        if (l.pColor == "White") a.color = Color.white;
        a.text = "ROOK";
        GameObject[] butts = GameObject.FindGameObjectsWithTag("skillbutt");
        Debug.Log(butts);
        foreach (GameObject butt in butts)
        {
            Destroy(butt);
        }
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            row1col1.GetComponent<variantchoso>().variant = new Rook();
            row1col2.GetComponent<variantchoso>().variant = new Rook();
            row1col3.GetComponent<variantchoso>().variant = new Rook();

            row2col1.GetComponent<variantchoso>().variant = new Rook();
            row2col2.GetComponent<variantchoso>().variant = new Rook();
            row2col3.GetComponent<variantchoso>().variant = new Rook();

            row3col1.GetComponent<variantchoso>().variant = new Rook();
            row3col2.GetComponent<variantchoso>().variant = new Rook();
            row3col3.GetComponent<variantchoso>().variant = new Rook();

            foreach (Piece cp in Rooklist)
            {
                if (cp != null)
                {
                    int a = Rooklist.IndexOf(cp);
                    if (a != -1)
                    {
                        if (a <= 3)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 4 && a <= 7)
                        {
                            row2[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 6 && a <= 9)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnQueenClicked()
    {
        if (l.pColor == "White") a.color = Color.white;
        a.text = "QUEEN";
        GameObject[] butts = GameObject.FindGameObjectsWithTag("skillbutt");
        Debug.Log(butts);
        foreach (GameObject butt in butts)
        {
            Destroy(butt);
        }
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            row1col1.GetComponent<variantchoso>().variant = new Queen();
            row1col2.GetComponent<variantchoso>().variant = new Queen();
            row1col3.GetComponent<variantchoso>().variant = new Queen();

            row2col1.GetComponent<variantchoso>().variant = new Queen();
            row2col2.GetComponent<variantchoso>().variant = new Queen();
            row2col3.GetComponent<variantchoso>().variant = new Queen();

            row3col1.GetComponent<variantchoso>().variant = new Queen();
            row3col2.GetComponent<variantchoso>().variant = new Queen();
            row3col3.GetComponent<variantchoso>().variant = new Queen();


            foreach (Piece cp in Queenlist)
            {
                if (cp != null)
                {
                    int a = Queenlist.IndexOf(cp);
                    if (a != -1)
                    {
                        if (a <= 3)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 4 && a <= 7)
                        {
                            row2[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 6 && a <= 9)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnKingClicked()
    {
        if (l.pColor == "White") a.color = Color.white;
        a.text = "KING";
        GameObject[] butts = GameObject.FindGameObjectsWithTag("skillbutt");
        Debug.Log(butts);
        foreach (GameObject butt in butts)
        {
            Destroy(butt);
        }
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            row1col1.GetComponent<variantchoso>().variant = new King();
            row1col2.GetComponent<variantchoso>().variant = new King();
            row1col3.GetComponent<variantchoso>().variant = new King();

            row2col1.GetComponent<variantchoso>().variant = new King();
            row2col2.GetComponent<variantchoso>().variant = new King();
            row2col3.GetComponent<variantchoso>().variant = new King();

            row3col1.GetComponent<variantchoso>().variant = new King();
            row3col2.GetComponent<variantchoso>().variant = new King();
            row3col3.GetComponent<variantchoso>().variant = new King();


            foreach (Piece cp in Kinglist)
            {
                if (cp != null)
                {
                    int a = Kinglist.IndexOf(cp);
                    if (a != -1)
                    {
                        if (a <= 3)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 4 && a <= 7)
                        {
                            row2[a].GetComponent<variantchoso>().variant = cp;
                        }
                        else if (a > 6 && a <= 9)
                        {
                            row1[a].GetComponent<variantchoso>().variant = cp;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void Update()
    {
        if (l.pColor == "White")
        {
            bg.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("whtie_bg");
            playerbox.GetComponent<Image>().color = Color.white;
            piecebox.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Character boxbl");
        }
        else
        {
            bg.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("black_bg");
        }
    }
}


