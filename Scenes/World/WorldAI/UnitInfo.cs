namespace DuchyOfThorns;

[GlobalClass]
public partial class UnitInfo : Resource
{
    [Export] public TroopType Type { get; set; }
    [Export] public int TroopsLeft { get; set; } = 0;
    [Export] public Stats Stats { get; set; }

    public UnitInfo Duplicate()
    {
        return new UnitInfo()
        {
            Type = Type,
            TroopsLeft = TroopsLeft,
        };
    }
}