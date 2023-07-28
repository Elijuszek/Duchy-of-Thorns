namespace DuchyOfThorns;

[GlobalClass]
public partial class UnitInfo : Resource
{
    [Export] public PackedScene Units { get; set; } = null;
    [Export] public int Count { get; set; } = 0;
    [Export] public Stats DefaultStats { get; set; } = null;
    
    public UnitInfo Duplicate()
    {
        return new UnitInfo()
        {
            Units = Units,
            Count = Count,
            DefaultStats = DefaultStats
        };
    }
}