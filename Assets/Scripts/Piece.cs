using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    public string suit;
    public int PV;
    public string move;
    public Chessman owner;
    public List<Ability> kit = new List<Ability>();
    public Sprite Skin;

    public Piece()
    {
        kit.Add(new Move());
        kit.Add(new Pass());
    }

    public virtual void Passive() { }
    public virtual void PreMove(string name) { }
    public virtual void PostMove(string name) { }
    public virtual void Counter(string name) { }
    public virtual void OnCapture(string name) { }

    public virtual Piece Clone()
    {
        Debug.Log("Cloning: Piece");
        return new Piece();
    }
}

public class Knight : Piece
{
    public Knight()
    {
        suit = "knight";
        PV = 300;
        move = "L-Move";
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

    public override Piece Clone()
    {
        Debug.Log("Cloning: Knight");
        return new Knight();
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

    public override Piece Clone()
    {
        Debug.Log("Cloning: Rook");
        return new Rook();
    }
}

public class Bishop : Piece
{
    public Bishop()
    {
        suit = "bishop";
        PV = 330;
        move = "DiagonalMove";
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

    public override Piece Clone()
    {
        Debug.Log("Cloning: Bishop");
        return new Bishop();
    }
}

public class Queen : Piece
{
    public Queen()
    {
        suit = "queen";
        PV = 900;
        move = "AnyDirectionMove";
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

    public override Piece Clone()
    {
        Debug.Log("Cloning: Queen");
        return new Queen();
    }
}

public class King : Piece
{
    public King()
    {
        suit = "king";
        PV = 10000;
        move = "OneStepAnyDirection";
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

    public override Piece Clone()
    {
        Debug.Log("Cloning: King");
        return new King();
    }
}

public class Pawn : Piece
{
    public Pawn()
    {
        suit = "pawn";
        PV = 100;
        move = "ForwardOneStep";
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

    public override Piece Clone()
    {
        Debug.Log("Cloning: Pawn");
        return new Pawn();
    }
}