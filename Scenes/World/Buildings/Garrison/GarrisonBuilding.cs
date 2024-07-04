using Godot.Collections;
namespace DuchyOfThorns;

public partial class GarrisonBuilding : EnterableBuilding
{
    [Export] protected GarrisonInfo GarrisonInfo { get; set; }
    [Export] protected Array<Marker2D> spawnPoints;
    [Export] private TroopsManager troopsManager;

    private bool CanSpawn = true;

    public void SpawnGarrison()
    {
        CanSpawn = true;
        for (int i = 0; i < GarrisonInfo.UnitQueue.Count; i++)
        {
            for (int j = 0; j < GarrisonInfo.UnitQueue[i].TroopsLeft; j++)
            {
                SpawnUnit(GarrisonInfo.UnitQueue[i], GarrisonInfo.UnitOrigins[i]);
            }
        }
    }

    private void SpawnUnit(UnitInfo unit, Vector2 origin)
    {
        Troop spawnedTroop = troopsManager.HandleTroopSpawned(unit.Type, unit.Stats, spawnPoints[Utilities.GetRandomInt(0, spawnPoints.Count-1)].GlobalPosition, origin);

        spawnedTroop.RemovedFromScene += (source) => HandleTroopRemoved(source, unit, origin);
    }

    private void HandleTroopRemoved(IPoolable source, UnitInfo unit, Vector2 origin)
    {
        source.RemovedFromScene -= (source) => HandleTroopRemoved(source, unit, origin);
        if (CanSpawn)
        {
            SpawnUnit(unit, origin);
        }
    }

    public void Clear()
    {
        CanSpawn = false;
        foreach (Troop troop in troopsManager.GetChildren().OfType<Troop>().Where(t => t.Team == Team.PLAYER))
        {
            if (troop.CurrentState != TroopState.NONE)
            {
                troop.RemoveFromScene();
            }
        }
    }
}
