using Godot.Collections;


namespace DuchyOfThorns;

[GlobalClass]
public partial class WaveInfo : Resource
{
    [Export] public int MaxUnits { get; set; } = 1;
    [Export] public Array<UnitInfo> UnitQueue { get; private set; }
    [Export] public double MaxDuration { get; set; } = 5000;
    [Export] public float Reward { get; set; } = 100;

    [Export] public int AttackDirections { get; set; } // Min 1, Max 4

    [Export] public float DayTime { get; set; } = 360;

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
        return new WaveInfo
        {
            MaxUnits = MaxUnits,
            UnitQueue = new Array<UnitInfo>(UnitQueue.Select(unit => unit.Duplicate())),
            MaxDuration = MaxDuration,
            Reward = Reward,
            DayTime = DayTime
        };
    }

    public TroopType DequeuUnit()
    {
        if ((UnitQueue.Count <= 0) || (UnitQueue.Count == 1 && UnitQueue[0].TroopsLeft <= 0))
        {
            return TroopType.NONE;
        }

        if (UnitQueue[0].TroopsLeft <= 0)
        {
            UnitQueue.RemoveAt(0);
        }

        TroopType type = UnitQueue[0].Type;
        UnitQueue[0].TroopsLeft--;
        return type;
    }
}

