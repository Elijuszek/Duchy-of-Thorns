using Godot.Collections;

namespace DuchyOfThorns;

/// <summary>
/// TODO: Rework capturableBaseSystem
/// WorldAI class which handles the spawning of units and the capturing of bases
/// </summary>
public partial class WorldAI : Node2D
{
	[Export] protected BaseCaptureOrder baseCaptureOrder = BaseCaptureOrder.FIRST;
	[Export] protected Team team = Team.NEUTRAL;
	[Export] protected Array<UnitInfo> unitsToSpawn;

	protected Random random;
    public override void _Ready()
    {
        base._Ready();
    }

	public virtual void SpawnUnit(PackedScene unit, Vector2 origin, Vector2 spawnCords)
	{
		Troop troop = unit.Instantiate<Troop>();
        troop.GlobalPosition = spawnCords;
        troop.Origin = origin;
		troop.Destination = origin;
		troop.SetState(TroopState.ADVANCE);
		troop.Connect("Died", new Callable(this, "HandleUnitDeath"));
        AddChild(troop);
    }
	public virtual void HandleUnitDeath()
	{

	}
	
	public virtual void Clear()
	{

	}
}
