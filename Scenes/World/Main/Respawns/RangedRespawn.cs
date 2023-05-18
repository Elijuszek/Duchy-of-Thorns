using Godot;

public partial class RangedRespawn : Respawn
{
    Ranged aliveUnit;
    CapturableBase nextBase;
    Vector2 nextBaseCord = Vector2.Zero;
    public override void _Ready()
    {
        base._Ready();
        nextBaseCord = this.GlobalPosition;
    }
    public override void SpawnUnit()
    {
        if (RespawnCount > 0 && Unit != null)
        {
            aliveUnit = (Ranged)Unit.Instantiate();
            AddChild(aliveUnit);
            aliveUnit.Ai.pathfinding = this.pathfinding;
            aliveUnit.Connect("Died", new Callable(this, "HandleUnitDeath"));
            aliveUnit.Ai.Origin = this.GlobalPosition;
            aliveUnit.Ai.NextBaseObject = nextBase;
            aliveUnit.Ai.NextBase = nextBaseCord;
            aliveUnit.Ai.SetState((int)(RangedAI.State.ADVANCE));

            RespawnCount--;
        }
        else
        {
            base.Clear();
            EmitSignal("OutOfTroops");
        }
    }
    public override void SetCapturableBase(CapturableBase nextBase, Vector2 nextBaseCord)
    {
        this.nextBase = nextBase;
        this.nextBaseCord = nextBaseCord;
        if (aliveUnit is null)
        {
            return;
        }
        aliveUnit.Ai.NextBaseObject = nextBase;
        aliveUnit.Ai.NextBase = nextBaseCord;
        aliveUnit.Ai.SetState((int)RangedAI.State.ADVANCE);
    }
    public override void Clear()
    {
        if (Unit is null)
        {
            return;
        }
        base.Clear();
        aliveUnit.QueueFree();
    }
}
