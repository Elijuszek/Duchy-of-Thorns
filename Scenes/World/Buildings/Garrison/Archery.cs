using Godot.Collections;

namespace DuchyOfThorns;

public partial class Archery : EnterableBuilding
{
    [Export] public GarrisonInfo GarrisonInfo { get; set; }
    [Export] private TroopsManager troopsManager;
    private void SpawnGarrison()
    {
        foreach (UnitInfo unit in GarrisonInfo.UnitQueue)
        {
            for (int i = 0; i < unit.TroopsLeft; i++)
            {
                SpawnUnit(unit.Type, GarrisonInfo.UnitOrigins[i]);
            }
        }
    }

    private void SpawnUnit(TroopType type, Vector2 origin)
    {
        if (type == TroopType.NONE)
        {
            return;
        }

    //    Troop spawnedTroop = troopsManager.HandleTroopSpawned(type, currentWave.UnitQueue[0].Stats,
    //new Vector2(Utilities.GetRandomFloat(200f, 400f), Utilities.GetRandomFloat(200f, 400f)),
    //cbase.GetDestination());
    }
}
