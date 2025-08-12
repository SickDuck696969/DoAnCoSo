using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Army", menuName = "Scriptable Objects/Army")]
public class Army : ScriptableObject
{
    public List<Piece> army = new List<Piece>();
    public void Mu()
    {
        foreach (var piece in army) { 
            Debug.Log(piece);
        }
    }
    private void OnEnable()
    {
        army.Clear();

        Pawn pawn = new Pawn();
        pawn.Skin = Resources.Load<Sprite>("chess-pawn-black");
        army.Add(pawn);

        Knight knight = new Knight();
        knight.Skin = Resources.Load<Sprite>("chess-knight-black");
        army.Add(knight);

        Bishop bishop = new Bishop();
        bishop.Skin = Resources.Load<Sprite>("chess-bishop-black");
        army.Add(bishop);

        Rook rook = new Rook();
        rook.Skin = Resources.Load<Sprite>("chess-rook-black");
        army.Add(rook);

        Queen queen = new Queen();
        queen.Skin = Resources.Load<Sprite>("chess-queen-black");
        army.Add(queen);

        King king = new King();
        king.Skin = Resources.Load<Sprite>("chess-king-black");
        army.Add(king);
    }
}
