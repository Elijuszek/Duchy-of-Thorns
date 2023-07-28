namespace DuchyOfThorns;

public partial class Globals : Node
{
    [Signal] public delegate void ProjectileFiredEventHandler(ProjectileType type, float damage, Team team, Vector2 position, Vector2 direction);
    [Signal] public delegate void CoinsDropedEventHandler(int coins, Marker2D position, bool explosive);
    [Signal] public delegate void TroopSpawnedEventHandler(TroopType type, Marker2D position, Stats stats);
}
