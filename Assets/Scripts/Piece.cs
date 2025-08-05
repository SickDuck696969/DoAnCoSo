using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Piece
{
    public string suit;
    public int PV;
    public string move;
    public Chessman owner;
    public List<Ability> kit = new List<Ability>();
    public Piece()
    {
        kit.Add(new Move());
    }
    public virtual void Passive() { }
    public virtual void PreMove(string name) { }
    public virtual void PostMove(string name) { }
    public virtual void Counter(string name) { }
    public virtual void OnCapture(string name) { }
    public virtual Piece Clone() { return new Piece(); }
}

public class Knight : Piece
{
    public Knight()
    {
        suit = "knight";
        PV = 300;
        move = "L-Move";
    }
}
public class Rook : Piece
{
    public Rook()
    {
        suit = "rook";
        PV = 500;
        move = "LineMove";
    }
}
public class MaskedKnight : Knight
{
    public MaskedKnight()
    {
        kit.Add(new TyphoonEngine());
        kit.Add(new Skid());
        
    }
    public override void Passive()
    {
        Ability loaded = new Passive();
        foreach (Ability ability in kit)
        {
            if (ability.type == "passive")
            {
                loaded = ability;
                loaded.owner = owner;
            }
        }
        loaded.action();
    }
    public override void PreMove(string name)
    {
        Ability loaded = new PostMove();
        foreach (Ability ability in kit)
        {
            if (ability.type == "premove" && ability.name == name)
            {
                loaded = ability;
                loaded.owner = owner;
            }
        }
        loaded.action();
    }
    public override void PostMove(string name)
    {
        Ability loaded = new PostMove();
        foreach (Ability ability in kit)
        {
            if (ability.type == "postmove" && ability.name == name)
            {
                loaded = ability;
                loaded.owner = owner;
            }
        }
        loaded.action();
    }
    public override void Counter(string name)
    {

    }
    public override void OnCapture(string name)
    {

    }
    public override Piece Clone()
    {
        return new MaskedKnight();
    }
}

public class VenomRook : Rook
{

}
