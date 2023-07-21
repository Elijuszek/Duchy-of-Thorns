using Godot.Collections;
namespace DuchyofThorns;

partial class WaveInfo : Resource
{
    [Export] public int MaxUnits { get; set; }
    [Export] public Array<PackedScene> Units { get; set; }
    [Export] public Array<int> UnitCounts { get; set; }
    [Export] public double MaxDuration { get; set; }
    [Export] public float Reward { get; set; }

    public WaveInfo()
    {
        MaxUnits = 0;
        Units = new Array<PackedScene>();
        UnitCounts = new Array<int>();
        MaxDuration = 0;
        Reward = 0;
    }
}

