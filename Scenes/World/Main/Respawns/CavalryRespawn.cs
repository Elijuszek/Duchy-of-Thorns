namespace DuchyOfThorns;

/// <summary>
/// Class for spawning cavalry units
/// </summary>
public partial class CavalryRespawn : Respawn
{
    public override void _Ready() => base._Ready();
    public override void SpawnUnit()
    {
        if (Unit != null)
        {
            Cavalry cavalryUnit = Unit.Instantiate() as Cavalry;
            AddChild(cavalryUnit);
            //rangedUnit.Ai.pathfinding = this.pathfinding;
            //rangedUnit.Connect("Died",new Callable(this,"HandleUnitDeath"));
            //rangedUnit.GlobalPosition = this.GlobalPosition;
            //rangedUnit.Ai.Origin = this.GlobalPosition;
            //rangedUnit.Ai.NextBase = this.GlobalPosition;
            //rangedUnit.Ai.SetState((int)(RangedAI.State.PATROL));
        }
        else
        {
            //base.Clear();
            //EmitSignal("OutOfTroops");
        }
    }

    public override void SetCapturableBase(Vector2 nextBaseCord)
    {
        throw new NotImplementedException();
    }
    public override void Clear()
    {
        //if (Unit is null)
        //{
        //    return;
        //}
        //base.Clear();
        //aliveUnit.Die();
    }
}
