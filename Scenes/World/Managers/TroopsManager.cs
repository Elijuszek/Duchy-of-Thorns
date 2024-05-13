using System.Collections.Generic;

namespace DuchyOfThorns;

public partial class TroopsManager : Node2D
{
    [Export] private int startingCount = 1;

    private Dictionary<TroopType, ObjectPool<Troop>> troopPool;
    public override void _Ready()
    {
        base._Ready();
        troopPool = new Dictionary<TroopType, ObjectPool<Troop>>()
        {
            { TroopType.ALLY_FOOTMAN,  new ObjectPool<Troop>(this,
                           ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Troops/Melee/FootmanAlly.tscn"), startingCount)},
            { TroopType.ALLY_ARCHER,  new ObjectPool<Troop>(this,
                           ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Troops/Ranged/ArcherAlly.tscn"), startingCount)},

            { TroopType.ENEMY_FOOTMAN,  new ObjectPool<Troop>(this,
                           ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Troops/Melee/FootmanEnemy.tscn"), startingCount)},
            { TroopType.ENEMY_ARCHER,  new ObjectPool<Troop>(this,
                           ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Troops/Ranged/ArcherEnemy.tscn"), startingCount)},
                        { TroopType.ENEMY_MAGE,  new ObjectPool<Troop>(this,
                           ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Troops/Ranged/MageEnemy.tscn"), startingCount)}
        };
    }
    public Troop HandleTroopSpawned(TroopType type, Stats stats, Vector2 spawnPosition, Vector2 origin)
    {
        Troop troop = troopPool[type].Take();
        troop.Stats = stats;
        troop.GlobalPosition = spawnPosition;
        troop.Origin = origin;
        float randomX = Utilities.GetRandomFloat(-10f, 10f);
        float randomY = Utilities.GetRandomFloat(-10f, 10f);
        troop.Destination = new Vector2(randomX, randomY) + origin;
        troop.SetState(TroopState.ADVANCE);

        return troop;
    }
}
