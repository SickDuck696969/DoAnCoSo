using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Army", menuName = "Scriptable Objects/Army")]
public class Army : ScriptableObject
{
    public List<Piece> army = new List<Piece>();
    public Army()
    {
        army.Add(new MaskedKnight());
    }
}
