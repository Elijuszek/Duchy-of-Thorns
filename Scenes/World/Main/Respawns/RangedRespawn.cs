namespace DuchyOfThorns;

/// <summary>
/// Class for spawning ranged units
/// </summary>
public partial class RangedRespawn : Respawn
{
    Ranged aliveUnit;
    Vector2 nextBaseCord = Vector2.Zero;
    public override void _Ready()
    {
        base._Ready();
        nextBaseCord = GlobalPosition;
    }
    public override void SpawnUnit()
    {
        if (RespawnCount > 0 && Unit != null)
        {
            aliveUnit = Unit.Instantiate<Ranged>();
            AddChild(aliveUnit);
            aliveUnit.Connect("Died", new Callable(this, "HandleUnitDeath"));
            aliveUnit.Destination = nextBaseCord;
            aliveUnit.SetState(TroopState.ADVANCE);

            RespawnCount--;
        }
        else
        {
            base.Clear();
            EmitSignal("OutOfTroops");
        }
    }
    public override void SetCapturableBase(Vector2 nextBaseCord)
    {
        this.nextBaseCord = nextBaseCord;
        if (aliveUnit is null)
        {
            return;
        }
        aliveUnit.Destination = nextBaseCord;
        aliveUnit.SetState(TroopState.ADVANCE);
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
