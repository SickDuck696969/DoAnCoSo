using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Army", menuName = "Scriptable Objects/Army")]
public class Army : ScriptableObject
{
    public List<Piece> whitearmy = new List<Piece>();
    public List<Piece> blackarmy = new List<Piece>();
    public List<Piece> roster = new List<Piece>();

    private void OnEnable()
    {
        blackarmy.Clear();
        whitearmy.Clear();

        // Black pieces
        AddPieceToArmy(blackarmy, new Pawn(), "chess-pawn-black");
        AddPieceToArmy(blackarmy, new Knight(), "chess-knight-black");
        AddPieceToArmy(blackarmy, new Bishop(), "chess-bishop-black");
        AddPieceToArmy(blackarmy, new Rook(), "chess-rook-black");
        AddPieceToArmy(blackarmy, new Queen(), "chess-queen-black");
        AddPieceToArmy(blackarmy, new King(), "chess-king-black");

        // White pieces
        AddPieceToArmy(whitearmy, new Pawn(), "chess-pawn-white");
        AddPieceToArmy(whitearmy, new Knight(), "chess-knight-white");
        AddPieceToArmy(whitearmy, new Bishop(), "chess-bishop-white");
        AddPieceToArmy(whitearmy, new Rook(), "chess-rook-white");
        AddPieceToArmy(whitearmy, new Queen(), "chess-queen-white");
        AddPieceToArmy(whitearmy, new King(), "chess-king-white");

        // White pieces
        AddPieceToArmy(roster, new Pawn(), null);
        AddPieceToArmy(roster, new Knight(), null);
        AddPieceToArmy(roster, new Bishop(), null);
        AddPieceToArmy(roster, new Rook(), null);
        AddPieceToArmy(roster, new Queen(), null);
        AddPieceToArmy(roster, new King(), null);
        AddPieceToArmy(roster, new MaskedKnight(), null);
    }

    public void AddPieceToArmy(List<Piece> armyList, Piece piece, string spritePath)
    {
        if(spritePath != null)
        {
            piece.Skin = Resources.Load<Sprite>(spritePath);
        }
        armyList.Add(piece);
    }

    public void PrintblackArmy()
    {
        Debug.Log("Black Army:");
        foreach (var piece in blackarmy)
        {
            Debug.Log(piece);
            Debug.Log(piece.Skin);
        }
    }
    public void PrintwhiteArmy() {

        Debug.Log("White Army:");
        foreach (var piece in whitearmy)
        {
            Debug.Log(piece);
            Debug.Log(piece.Skin);
        }
    }
}
