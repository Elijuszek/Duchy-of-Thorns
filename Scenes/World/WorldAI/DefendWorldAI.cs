using Godot.Collections;

namespace DuchyOfThorns;

// TODO: Sepetate from AssaultWorldAI & implement defend WorldAI

/// <summary>
/// WorldAI which is dedicated for defend maps, acts as a garrison manager
/// </summary>
public partial class DefendWorldAI : Node2D
{
	[Export] protected BaseCaptureOrder baseCaptureOrder = BaseCaptureOrder.FIRST;
	[Export] protected Team team = Team.PLAYER;
	[Export] protected Array<GarrisonInfo> unitsToSpawn;
	[Export] protected TroopsManager troopsManager;

	public void SpawnUnit()
	{
		GarrisonInfo garrison = unitsToSpawn[0].Duplicate();
		TroopType type = garrison.DequeuUnit();
		for (int i = 0; i < garrison.MaxUnits; i++)
		{
			troopsManager.HandleTroopSpawned(type, garrison.UnitQueue[0].Stats,
				new Vector2(Utilities.GetRandomFloat(800f, 1200f), Utilities.GetRandomFloat(500f, 800f)),
				new Vector2(Utilities.GetRandomFloat(400f, 800f), Utilities.GetRandomFloat(400f, 600f)));
		}
	}
	
	public void Clear()
	{

	}
}
