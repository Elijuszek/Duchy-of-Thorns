namespace DuchyOfThorns;

/// <summary>
/// Class for the map which is used in the Capture the Base game mode
/// </summary>
public partial class CaptureMap : World
{
    private WorldAI enemyMapAI;
    private WorldAI allyMapAI;
    private CapturableBaseManager capturableBaseManager;
    public override void _Ready()
    {
        base._Ready();
        capturableBaseManager = GetNode<CapturableBaseManager>("CapturableBasesManager");
        enemyMapAI = GetNode<WorldAI>("EnemyMapAI");
        allyMapAI = GetNode<WorldAI>("AllyMapAI");
        //CapturableBase[] bases = capturableBaseManager.GetCapturableBases();

        Respawn[] allyRespawnPoints = GetNode<Node2D>("AllyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();
        Respawn[] enemyRespawnPoints = GetNode<Node2D>("EnemyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();


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
