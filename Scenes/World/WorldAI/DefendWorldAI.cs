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
		for (int i = 0; i < unitsToSpawn.Count; i++)
		{
			GarrisonInfo garrison = unitsToSpawn[i].Duplicate();
			TroopType type = garrison.DequeuUnit();
            troopsManager.HandleTroopSpawned(type, garrison.UnitQueue[0].Stats,
				new Vector2(Utilities.GetRandomFloat(800f, 1200f), Utilities.GetRandomFloat(800f, 1200f)),
				new Vector2(Utilities.GetRandomFloat(400f, 800f), Utilities.GetRandomFloat(400f, 800f)));
        }
    }
	
	public void Clear()
	{

	}
}
