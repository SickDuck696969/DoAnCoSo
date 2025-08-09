using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Army", menuName = "Scriptable Objects/Army")]
public class Army : ScriptableObject
{
    public List<Piece> army = new List<Piece>();
    public Army()
    {
        army.Add(new Pawn());
        army.Add(new Knight());
        army.Add(new Bishop());
        army.Add(new Rook());
        army.Add(new Queen());
        army.Add(new King());
    }
}
