using Godot.Collections;

namespace DuchyOfThorns;

[GlobalClass]
public partial class GarrisonInfo : Resource
{
    //[Export] public int Manpower { get; set; }
    [Export] public Array<UnitInfo> UnitGroups { get; private set; }
    [Export] public Array<Vector2> UnitOrigins { get; private set; }

    public GarrisonInfo()
    {
        UnitGroups = new Array<UnitInfo>();
        UnitOrigins = new Array<Vector2>();
    }
    public GarrisonInfo Duplicate()
    {
        return new GarrisonInfo()
        {
            UnitGroups = UnitGroups.Duplicate(),
            UnitOrigins = UnitOrigins
        };
    }
}
