using Godot.Collections;

namespace DuchyOfThorns;

// TODO: Sepetate from AssaultWorldAI & implement defend WorldAI

/// <summary>
/// WorldAI which is dedicated for defend maps, acts as a garrison manager
/// </summary>
public partial class DefendWorldAI : Node2D
{
	[Export] protected BaseCaptureOrder baseCaptureOrder = BaseCaptureOrder.FIRST;
	[Export] protected Array<GarrisonBuilding> garrisons;
	public void SpawnAllGarrison()
	{
		foreach(GarrisonBuilding garrison in garrisons)
		{
            garrison.SpawnGarrison();
        }
	}

	public void ClearWorld()
	{
        foreach (GarrisonBuilding garrison in garrisons)
        {
            garrison.Clear();
        }
    }
}
