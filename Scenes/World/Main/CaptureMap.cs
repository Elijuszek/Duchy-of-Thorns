namespace DuchyOfThorns;

/// <summary>
/// Class for the map which is used in the Capture the Base game mode
/// </summary>
public partial class CaptureMap : Map
{
    private Pathfinding pathfinding;
    private MapAI enemyMapAI;
    private MapAI allyMapAI;
    private CapturableBaseManager capturableBaseManager;
    public override void _Ready()
    {
        base._Ready();
        pathfinding = GetNode<Pathfinding>("PathFinding");
        pathfinding.CreateNavigationMap(ground);
        capturableBaseManager = GetNode<CapturableBaseManager>("CapturableBasesManager");
        enemyMapAI = GetNode<MapAI>("EnemyMapAI");
        allyMapAI = GetNode<MapAI>("AllyMapAI");
        CapturableBase[] bases = capturableBaseManager.GetCapturableBases();

        Respawn[] allyRespawnPoints = GetNode<Node2D>("AllyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();
        Respawn[] enemyRespawnPoints = GetNode<Node2D>("EnemyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();

        allyMapAI.Initialize(bases, allyRespawnPoints, pathfinding);
        enemyMapAI.Initialize(bases, enemyRespawnPoints, pathfinding);

        capturableBaseManager.Connect("PlayerCapturedAllBases", new Callable(this, "HandlePlayerVictory"));
        capturableBaseManager.Connect("PlayerLostAllBases", new Callable(this, "HandlePlayerDefeat"));

    }
    private void HandlePlayerVictory()
    {
        GameOverScreen gameOver = gameOverScene.Instantiate() as GameOverScreen;
        AddChild(gameOver);
        gameOver.SetTitle(true);
        gameOver.player = GetNode<Player>("Player");
        GetTree().Paused = true;
    }
    private void HandlePlayerDefeat()
    {
        GameOverScreen gameOver = gameOverScene.Instantiate() as GameOverScreen;
        AddChild(gameOver);
        gameOver.SetTitle(false);
        GetTree().Paused = true;
    }
}
