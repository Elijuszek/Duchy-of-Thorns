namespace DuchyOfThorns;

[GlobalClass]
public partial class UnitInfo : Resource
{
    [Export] public TroopType Type { get; set; }
    [Export] public int Count { get; set; } = 0;
    
    public UnitInfo Duplicate()
    {
        return new UnitInfo()
        {
            Type = Type,
            Count = Count,
        };
    }
}