using Godot.Collections;

namespace DuchyOfThorns;

[GlobalClass]
public partial class GarrisonInfo : Resource
{
    [Export] public int Manpower { get; set; }
    [Export] public int MaxUnits { get; set; }
    [Export] public Array<UnitInfo> UnitQueue { get; private set; }
    [Export] public Array<Vector2> UnitOrigins { get; private set; }

    public GarrisonInfo()
    {
        UnitQueue = new Array<UnitInfo>();
        UnitOrigins = new Array<Vector2>();
    }
    public GarrisonInfo Duplicate()
    {
        return new GarrisonInfo()
        {
            Manpower = Manpower,
            MaxUnits = MaxUnits,
            UnitQueue = new Array<UnitInfo>(UnitQueue.Select(unit => unit.Duplicate())),
            UnitOrigins = UnitOrigins
        };
    }

    public TroopType DequeuUnit()
    {
        if ((UnitQueue.Count <= 0) || (UnitQueue.Count == 1 && UnitQueue[0].TroopsLeft <= 0) || Manpower == 0)
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
