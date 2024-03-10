using Godot.Collections;

namespace DuchyOfThorns;

// TODO: Sepetate from AssaultWorldAI & implement defend WorldAI

/// <summary>
/// WorldAI which is dedicated for defend maps, acts as a garrison manager
/// </summary>
public partial class DefendWorldAI : Node2D
{
	[Export] protected BaseCaptureOrder baseCaptureOrder = BaseCaptureOrder.FIRST;
	[Export] protected Team team = Team.NEUTRAL;
	[Export] protected Array<UnitInfo> unitsToSpawn;
	[Export] protected TroopsManager troopsManager;

	public virtual void SpawnUnit(PackedScene unit, Vector2 origin, Vector2 spawnCords)
	{

    }
	
	public virtual void Clear()
	{

	}
}
