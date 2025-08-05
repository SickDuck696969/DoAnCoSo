using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class Ability
{
    public string name;
    public string type;
    public int AP;
    public string desc;
    public Chessman owner;
    public virtual void end()
    {
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
    }
    public override void action()
    {
        if(!owner.hasMoved.Value && !owner.hasSpelled.Value)
        {
            locked = false;
        }
        if(!locked && owner.hasMoved.Value)
        {
            if(owner.player.Value == "Black")
            {
                owner.controller.GetComponent<Game>().black_AP += 150;
            } else if (owner.player.Value == "White")
            {
                owner.controller.GetComponent<Game>().white_AP += 150;
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
    }
    public override void action()
    {
        Game sc = owner.controller.GetComponent<Game>();
        int x = owner.xBoard.Value * owner.xmodifier.Value;
        int y = owner.yBoard.Value * owner.ymodifier.Value;
        Debug.Log(x + " " + y);
        Debug.Log(x+1);
        Debug.Log((x-1) + " " + (y-2));
        Debug.Log(sc.GetPos(x - 1, y - 2));
        if (sc.GetPos(x-1, y-2) == null)
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
                owner.MovePlateAttackSpawnServerRpc(victim.xBoard.Value, victim.yBoard.Value);
                owner.hasMoved.Value = false;
                end();
            }
        }
    }
}
public class KnightKick : PreMove
{
    public KnightKick()
    {
        name = "Knight Kick";
        AP = 2200;
    }
    public override void action()
    {
        Game sc = owner.controller.GetComponent<Game>();
        int x = owner.xBoard.Value;
        int y = owner.yBoard.Value;
        owner.hasMoved.Value = true;
        for (int i = 1; i < 4; i++)
        {
            Debug.Log(i);
            Debug.Log(sc.GetPos(x, y + i));
            if (sc.GetPos(x, y + i) != null && sc.GetPos(x, y + i).GetComponent<Chessman>().player.Value != owner.player.Value)
            {
                owner.MovePlateAttackSpawnServerRpc(x, y + i);
            }
            else if(sc.GetPos(x,y + i) == null)
            {
                owner.MovePlateSpawnServerRpc(x, y + i);
            }
        }
        end();
    }
}
public class ChoHenshin : PreMove
{
    public ChoHenshin()
    {
        name = "Cho Henshin";
        AP = 1250;
    }
}

public class Move : PreMove
{
    public Move()
    {
        name = "Move";
    }
    public override void action()
    {
        owner.hasMoved.Value = true;
        owner.InitiateMovePlates();
    }
}