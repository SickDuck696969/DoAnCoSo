using JetBrains.Annotations;
using System;
using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class Ability
{
    public string name = "";
    public string type = "";
    public int AP = 0;
    public string desc = "";
    public Chessman owner;
    public virtual void end()
    {
        owner.killbutt();
        owner.hasSpelled.Value = true;
        owner.controller.GetComponent<Game>().APloseServerRpc(owner.player.Value.ToString(), AP);
    }
    public virtual void action() { }
}
public class Passive : Ability
{
    public Passive()
    {
        type = "passive";
    }
}

public class PreMove : Ability
{
    public PreMove()
    {
        type = "premove";
    }
    public override void end()
    {
        owner.controller.GetComponent<Game>().APloseServerRpc(owner.player.Value.ToString(), AP);
    }
}

public class PostMove : Ability
{
    public PostMove()
    {
        type = "postmove";
    }
}

public class Counter : Ability
{
    public Counter()
    {
        type = "counter";
    }
}

public class OnCapture : Ability
{
    public OnCapture()
    {
        type = "oncapture";
    }
}
public class TyphoonEngine : Passive
{
    public bool locked = false;
    public TyphoonEngine()
    {
        name = "Typoon Engine";
        desc = "gain 50AP everytime you move";
    }
    public override void action()
    {
        if(!owner.hasMoved.Value && !owner.hasSpelled.Value && owner.player.Value.ToString() == owner.controller.GetComponent<Game>().currentPlayer.Value.ToString())
        {
            Debug.Log(owner.controller.GetComponent<Game>().currentColor.pColor);
            locked = false;
        }
        if(!locked && owner.hasMoved.Value)
        {
            if(owner.player.Value == "Black")
            {
                owner.controller.GetComponent<Game>().APloseServerRpc("Black", -150);
            } else if (owner.player.Value == "White")
            {
                owner.controller.GetComponent<Game>().APloseServerRpc("White", -150);
            }
            locked = true;
        }
    }
}

public class Skid : PostMove
{
    public Skid()
    {
        name = "Skid";
        AP = 1100;
        desc = "After moving your L-Move if theres a piece diagnally infront of you you get to move in and capture it.";
    }
    public override void action()
    {
        Debug.Log("usinganacctikon");
        Game sc = owner.controller.GetComponent<Game>();
        int x = owner.xBoard.Value * owner.xmodifier.Value;
        int y = owner.yBoard.Value * owner.ymodifier.Value;
        Debug.Log(x + " " + y);
        Debug.Log(x+1);
        Debug.Log((x-1) + " " + (y-2));
        try
        {
            if (x - 1 < sc.positions.Length && y - 2 < sc.positions.Length && sc.GetPos(x - 1, y - 2).name.EndsWith(owner.variant.suit))
            {
                Chessman victim = new Chessman();
                if (sc.GetPos(x + 1, y) != null)
                {
                    victim = sc.GetPos(x + 1, y).GetComponent<Chessman>();
                }
                else
                {
                    owner.hasMoved.Value = true;
                    end();
                }
                if (victim != null && victim.player != owner.player)
                {
                    owner.MovePlateAttackSpawnServerRpc(victim.xBoard.Value, victim.yBoard.Value, 0);
                    owner.hasMoved.Value = false;
                    end();
                }
            }
        }
        catch (IndexOutOfRangeException ex)
        {
            Debug.Log("out o range");
        }

    }
}
public class Pass : PostMove
{
    public Pass()
    {
        name = "Pass";
        AP = 0;
        desc = "pass";
    }
    public override void action()
    {
        end();
    }
}
public class KnightKick : PreMove
{
    public KnightKick()
    {
        name = "Knight Kick";
        AP = 2200;
        desc = "Attack any one of 4 squares in  front of you except ones already occupied by an ally piece, the landing have residue damage on adjacent pieces enemy or ally equal to your PV - victim PV/10. ";
    }
    public override void action()
    {
        Debug.Log("lsad");
        Game sc = owner.controller.GetComponent<Game>();
        int x = owner.xBoard.Value;
        int y = owner.yBoard.Value;
        owner.hasMoved.Value = true;
        owner.killbutt();
        owner.EffectSpawnServerRpc(0, x, y);
        end();
    }
}
public class ChoHenshin : PreMove
{
    public ChoHenshin()
    {
        name = "Cho Henshin";
        AP = 1250;
        desc = "henshin";
    }
}

public class Move : PreMove
{
    public Move()
    {
        name = "Move";
        desc = "move";
    }
    public override void action()
    {
        owner.hasMoved.Value = true;
        Debug.Log(owner.variant);
        owner.killbutt();
        owner.InitiateMovePlates();
    }
}