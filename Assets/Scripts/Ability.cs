using System;
using System.Diagnostics;
using Unity.Netcode;

public class Ability
{
    public string name = "";
    public string type = "";
    public int AP = 0;
    public string desc = "";
    public Chessman owner;
    public virtual void end()
    {
        owner.killbuttClientRpc();
        owner.hasSpelled.Value = true;
        owner.controller.GetComponent<Game>().APloseServerRpc(owner.player.Value.ToString(), AP);
    }
    [ServerRpc()]
    public virtual void action() { }
    public virtual Ability Clone()
    {
        Ability ability = new Ability();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        return ability;
    }
}
public class Passive : Ability
{
    public Passive()
    {
        type = "passive";
    }
    public override Ability Clone()
    {
        Passive ability = new Passive();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        return ability;
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
    public override Ability Clone()
    {
        PreMove ability = new PreMove();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        ability.AP = AP;
        return ability;
    }
}

public class PostMove : Ability
{
    public PostMove()
    {
        type = "postmove";
    }
    public override Ability Clone()
    {
        PostMove ability = new PostMove();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        ability.AP = AP;
        return ability;
    }
}

public class Counter : Ability
{
    public Counter()
    {
        type = "counter";
    }
    public override Ability Clone()
    {
        Counter ability = new Counter();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        ability.AP = AP;
        return ability;
    }
}

public class OnCapture : Ability
{
    public OnCapture()
    {
        type = "oncapture";
    }
    public override Ability Clone()
    {
        OnCapture ability = new OnCapture();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        ability.AP = AP;
        return ability;
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
        if (!owner.hasMoved.Value && !owner.hasSpelled.Value && owner.player.Value.ToString() == owner.controller.GetComponent<Game>().currentPlayer.Value.ToString())
        {
            locked = false;
        }
        if (!locked && owner.hasMoved.Value)
        {
            if (owner.player.Value == "Black")
            {
                owner.controller.GetComponent<Game>().APloseServerRpc("Black", -150);
            }
            else if (owner.player.Value == "White")
            {
                owner.controller.GetComponent<Game>().APloseServerRpc("White", -150);
            }
            locked = true;
        }
    }
    public override Ability Clone()
    {
        TyphoonEngine ability = new TyphoonEngine();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        return ability;
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
        Game sc = owner.controller.GetComponent<Game>();
        int x = owner.xBoard.Value * owner.xmodifier.Value;
        int y = owner.yBoard.Value * owner.ymodifier.Value;
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
            Debug.WriteLine(ex.Message);
        }
    }
    public override Ability Clone()
    {
        Skid ability = new Skid();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        ability.AP = AP;
        return ability;
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
    public override Ability Clone()
    {
        Pass ability = new Pass();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        ability.AP = AP;
        return ability;
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
        Game sc = owner.controller.GetComponent<Game>();
        int x = owner.xBoard.Value;
        int y = owner.yBoard.Value;
        owner.hasMoved.Value = true;
        owner.killbuttClientRpc();
        for (int i = 1; i < 3; i++)
        {
            int mod = owner.player.Value == "White" ? 1 : -1;
            int targetY = x + (i * mod);
            var pos = sc.GetPos(x, targetY);
            if (pos != null && pos.GetComponent<Chessman>().player.Value != owner.player.Value)
            {
                owner.MovePlateAttackSpawnServerRpc(x, targetY, 2);
            }
            else if (pos == null)
            {
                owner.MovePlateSpawnServerRpc(x, targetY, 2);
            }
        }
        end();
    }
    public override Ability Clone()
    {
        KnightKick ability = new KnightKick();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        ability.AP = AP;
        return ability;
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
    public override Ability Clone()
    {
        ChoHenshin ability = new ChoHenshin();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        ability.AP = AP;
        return ability;
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
        owner.killbuttClientRpc();
        owner.InitiateMovePlates();
    }
    public override Ability Clone()
    {
        Move ability = new Move();
        ability.name = name;
        ability.type = type;
        ability.desc = desc;
        ability.AP = AP;
        return ability;
    }
}
