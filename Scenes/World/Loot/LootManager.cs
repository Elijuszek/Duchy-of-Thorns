using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuchyOfThorns;

/// <summary>
/// Class for managing loot spawning and pooling
/// </summary>
public partial class LootManager : Node2D
{
    [Export] private int coinsCount = 20;

    private Dictionary<LootType, ObjectPool<Coin>> lootPool;
    public override void _Ready()
    {
        base._Ready();
        lootPool = new Dictionary<LootType, ObjectPool<Coin>>()
        {
            { LootType.GOLD,  new ObjectPool<Coin>(this,
                ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Coins/GoldenCoin.tscn"), coinsCount)},

            { LootType.SILVER,  new ObjectPool<Coin>(this,
                ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Coins/SilverCoin.tscn"), coinsCount)},

            { LootType.BRONZE,  new ObjectPool<Coin>(this,
                ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Coins/BronzeCoin.tscn"), coinsCount)}
        };

        // For debugging purposes
        HandleCoinsSpawned(9999, new Vector2(400, 400), true);
    }
    public void HandleCoinsSpawned(int coins, Vector2 position, bool explosive)
    {
        int[] count = new int[3];
        count[0] = coins / 100; // gold
        coins -= count[0] * 100;
        count[1] = coins / 10;  // silver
        coins -= count[1] * 10;
        count[2] = coins;       // bronze
        if (explosive)
        {
            for (int i = 0; i < count.Length; i++)
            {
                for (int j = 0; j < count[i]; j++)
                {
                    Vector2 direction = new Vector2(Globals.GetRandomFloat(-15, 15), Globals.GetRandomFloat(-15, 15));
                    Coin coin = lootPool[(LootType)i].Take();
                    coin.GlobalPosition = position;
                    coin.Move(position + direction);
                }
            }
            return;
        }
        for (int i = 0; i < count.Length; i++)
        {
            for (int j = 0; j < count[i]; j++)
            {
                Coin coin = lootPool[(LootType)i].Take();
                coin.GlobalPosition = position;
            }
        }
    }
}
