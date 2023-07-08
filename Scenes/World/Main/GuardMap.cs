namespace DuchyOfThorns;

/// <summary>
/// Class for the guard map, without capturable bases
/// </summary>
public partial class GuardMap : Map
{
    protected Respawn[] allyRespawnPoints;
    protected Respawn[] enemyRespawnPoints;

    public override void _Ready()
    {
        base._Ready();

        allyRespawnPoints = GetNode<Node2D>("AllyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();
        enemyRespawnPoints = GetNode<Node2D>("EnemyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();

        Initialize();

    }

    private void Initialize()
    {
        foreach (Respawn respawn in allyRespawnPoints)
        {
            respawn.SpawnUnit();
        }
        foreach (Respawn respawn in enemyRespawnPoints)
        {
            respawn.SpawnUnit();
        }
    }
}
