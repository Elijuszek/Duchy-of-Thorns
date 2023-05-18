using Godot;
using System;
public partial class LootManager : Node2D
{
    [Export] private int goldenCount = 20;
    [Export] private int silverCount = 100;
    private PackedScene goldenCoinScene;
    private PackedScene silverCoinScene;
    private PackedScene bronzeCoinScene;
    private ObjectPool<Coin> goldenCoinsPool;
    private ObjectPool<Coin> silverCoinsPool;
    private ObjectPool<Coin> bronzeCoinsPool;
    Random rand;
    public override void _Ready()
    {
        base._Ready();
        goldenCoinScene = (PackedScene)ResourceLoader.Load("res://Scenes/Actors/Coins/GoldenCoin.tscn");
        silverCoinScene = (PackedScene)ResourceLoader.Load("res://Scenes/Actors/Coins/SilverCoin.tscn");
        bronzeCoinScene = (PackedScene)ResourceLoader.Load("res://Scenes/Actors/Coins/BronzeCoin.tscn");
        rand = new Random();

        goldenCoinsPool = new ObjectPool<Coin>();
        silverCoinsPool = new ObjectPool<Coin>();
        bronzeCoinsPool = new ObjectPool<Coin>();
        for (int i = 0; i < goldenCount; i++)
        {
            Coin temp = goldenCoinScene.Instantiate() as Coin;
            temp.Connect("CoinRemoved", new Callable(this, "ReleaseGoldenCoin"));
            AddChild(temp);
            temp.RemoveFromScene();
        }
        for (int i = 0; i < silverCount; i++)
        {
            Coin temp = silverCoinScene.Instantiate() as Coin;
            temp.Connect("CoinRemoved", new Callable(this, "ReleaseSilverCoin"));
            AddChild(temp);
            temp.RemoveFromScene();

            temp = bronzeCoinScene.Instantiate() as Coin;
            temp.Connect("CoinRemoved", new Callable(this, "ReleaseBronzeCoin"));
            AddChild(temp);
            temp.RemoveFromScene();
        }
        HandleCoinsSpawned(1, new Vector2(400, 400));
    }
    public void HandleCoinsSpawned(int coins, Vector2 position)
    {
        Coin temp;
        for (int i = 0; i < 2; i++)
        {
            if (coins >= 50)
            {
                temp = goldenCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 50;
            }
            else if (coins >= 10)
            {
                temp = silverCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 10;
            }
            else if (coins > 0)
            {
                temp = bronzeCoinsPool.Get();
                temp.GlobalPosition = position;
                coins -= 1;
            }
            else
            {
                return;
            }
            temp.AddToScene();
        }
        if (coins >= 50)
        {
            temp = goldenCoinsPool.Get();
            temp.Gold = coins;
            temp.GlobalPosition = position; ;
        }
        else if (coins >= 10)
        {
            temp = silverCoinsPool.Get();
            temp.Gold = coins;
            temp.GlobalPosition = position;
        }
        else if (coins > 0)
        {
            temp = bronzeCoinsPool.Get();
            temp.Gold = coins;
            temp.GlobalPosition = position;
        }
    }
    private void ReleaseGoldenCoin(Coin coin)
    {
        coin.Gold = 50;
        goldenCoinsPool.Release(coin);
    }
    private void ReleaseSilverCoin(Coin coin)
    {
        coin.Gold = 10;
        silverCoinsPool.Release(coin);
    }
    private void ReleaseBronzeCoin(Coin coin)
    {
        coin.Gold = 1;
        bronzeCoinsPool.Release(coin);
    }
}
