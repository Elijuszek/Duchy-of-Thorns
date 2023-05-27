using Godot.Collections;
namespace DuchyOfThorns;

/// <summary>
/// TODO: should be replaced using resource material
/// Class for storing wave data
/// </summary>
public partial class Wave : Node
{
    [Export] public int Reward { get; set; } // extra reward for surviving wave
    [Export] public double Duration { get; set; } // wave duration in seconds

    // Infantry
    [Export] private PackedScene[] infantryUnits;
    [Export] private Array<int> infantryCount;
    [Export] private Array<float> infantryCooldown;
    public PackedScene GetInfantryUnits(int index) => infantryUnits[index];
    public int GetInfantryRespawnCount(int index) => infantryCount[index];
    public float GetInfantryCooldown(int index) => infantryCooldown[index];
    public int GetInfantryLength() => infantryUnits.Length;

    // Ranged
    [Export] private PackedScene[] rangedUnits;
    [Export] private Array<int> rangedCount;
    [Export] private Array<float> rangedCooldown;
    public PackedScene GetRangedUnits(int index) => rangedUnits[index];
    public int GetRangedRespawnCount(int index) => rangedCount[index];
    public float GetRangedCooldown(int index) => rangedCooldown[index];
    public int GetRangedLength() => rangedUnits.Length;

    // Cavalry
    [Export] private PackedScene[] cavalryUnits = new PackedScene[5];
    [Export] private Array<int> cavalryCount;
    [Export] private Array<float> cavalryCooldown;
    public PackedScene GetCavalryUnits(int index)
    => cavalryUnits[index];
    public int GetCavalryRespawnCount(int index) => cavalryCount[index];
    public float GetCavalryCooldown(int index) => cavalryCooldown[index];
    public int GetCavalryLength() => cavalryUnits.Length;
}
