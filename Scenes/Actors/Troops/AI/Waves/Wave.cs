using Godot;
using Godot.Collections;

public partial class Wave : Node
{
    [Export] public int Reward { get; set; } // extra reward for surviving wave
    [Export] public double Duration { get; set; } // wave duration in seconds
                                                  // Infantry
    [Export] private PackedScene[] infantryUnits;
    [Export] private Array<int> infantryCount;
    [Export] private Array<float> infantryCooldown;
    public PackedScene GetInfantryUnits(int index)
    {
        return infantryUnits[index];
    }
    public int GetInfantryRespawnCount(int index)
    {
        return infantryCount[index];
    }
    public float GetInfantryCooldown(int index)
    {
        return infantryCooldown[index];
    }
    public int GetInfantryLength()
    {
        return infantryUnits.Length;
    }

    // Ranged
    [Export] private PackedScene[] rangedUnits;
    [Export] private Array<int> rangedCount;
    [Export] private Array<float> rangedCooldown;
    public PackedScene GetRangedUnits(int index)
    {
        return rangedUnits[index];
    }
    public int GetRangedRespawnCount(int index)
    {
        return rangedCount[index];
    }
    public float GetRangedCooldown(int index)
    {
        return rangedCooldown[index];
    }
    public int GetRangedLength()
    {
        return rangedUnits.Length;
    }

    // Cavalry
    [Export] private PackedScene[] cavalryUnits = new PackedScene[5];
    [Export] private Array<int> cavalryCount;
    [Export] private Array<float> cavalryCooldown;
    public PackedScene GetCavalryUnits(int index)
    {
        return cavalryUnits[index];
    }
    public int GetCavalryRespawnCount(int index)
    {
        return cavalryCount[index];
    }
    public float GetCavalryCooldown(int index)
    {
        return cavalryCooldown[index];
    }
    public int GetCavalryLength()
    {
        return cavalryUnits.Length;
    }
}
