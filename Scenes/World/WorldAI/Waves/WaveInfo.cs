using Godot.Collections;
namespace DuchyOfThorns;

[GlobalClass]
public partial class WaveInfo : Resource
{
    [Export] public int MaxUnits { get; set; }
    [Export] public Array<UnitInfo> UnitQueue { get; private set; }
    [Export] public double MaxDuration { get; set; }
    [Export] public float Reward { get; set; }

    [Export] public int AttackDirections { get; set; } // Min 1, Max 4

    public WaveInfo()
    {
        MaxUnits = 0;
        UnitQueue = new Array<UnitInfo>();
        MaxDuration = 0;
        Reward = 0;
        AttackDirections = 0;
    }
    public WaveInfo Duplicate()
    {
        return new WaveInfo()
        {
            MaxUnits = MaxUnits,
            UnitQueue = UnitQueue.Duplicate(),
            MaxDuration = MaxDuration,
            Reward = Reward
        };
    }
}

