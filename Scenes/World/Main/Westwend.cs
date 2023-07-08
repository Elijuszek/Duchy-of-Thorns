namespace DuchyOfThorns;

/// <summary>
/// Final map class
/// </summary>
public partial class Westwend : Node2D
{
    private PackedScene gameOverScene;
    private PackedScene playerScene;
    private PackedScene pauseMenuScene;
    private PackedScene deathScreenScene;
    private Globals globals;
    private ProjectileManager projectileManager;
    private Marker2D playerSpawn;
    private Camera2D camera;
    private GUI gui;
    private TileMap ground;
    private CapturableBaseManager capturableBaseManager;
    private LootManager lootManager;
    private MapAI enemyMapAI;
    private MapAI allyMapAI;
    public override void _Ready()
    {
        ground = GetNode<TileMap>("Ground");
        camera = GetNode<Camera2D>("Camera2D");
        gameOverScene = (PackedScene)ResourceLoader.Load("res://Scenes/UI/GameOverScreen.tscn");
        playerScene = (PackedScene)ResourceLoader.Load("res://Scenes/Actors/Characters/Player/Player.tscn");
        pauseMenuScene = (PackedScene)ResourceLoader.Load("res://Scenes/UI/PauseScreen.tscn");
        deathScreenScene = (PackedScene)ResourceLoader.Load("res://Scenes/UI/DeathScreen.tscn");

        lootManager = GetNode<LootManager>("LootManager");
        projectileManager = GetNode<ProjectileManager>("ProjectileManager");

        globals = GetNode<Globals>("/root/Globals");

        playerSpawn = GetNode<Marker2D>("PlayerSpawn");
        gui = GetNode<GUI>("GUI");

        capturableBaseManager = GetNode<CapturableBaseManager>("CapturableBasesManager");

        enemyMapAI = GetNode<MapAI>("EnemyMapAI");
        allyMapAI = GetNode<MapAI>("AllyMapAI");

        globals.Connect("ArrowFired", new Callable(projectileManager, "HandleArrowSpawned"));
        globals.Connect("CoinsDroped", new Callable(lootManager, "HandleCoinsSpawned"));


        CapturableBase[] bases = capturableBaseManager.GetCapturableBases();

        Respawn[] allyRespawnPoints = GetNode<Node2D>("AllyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();
        Respawn[] enemyRespawnPoints = GetNode<Node2D>("EnemyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();

        allyMapAI.Initialize(bases, allyRespawnPoints);
        enemyMapAI.Initialize(bases, enemyRespawnPoints);

        capturableBaseManager.Connect("PlayerCapturedAllBases", new Callable(this, "HandlePlayerVictory"));
        capturableBaseManager.Connect("PlayerLostAllBases", new Callable(this, "HandlePlayerDefeat"));

        SetCameraLimits();
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Player player = playerScene.Instantiate() as Player;
        player.Position = playerSpawn.Position;
        AddChild(player);
        player.SetCameraTransform(camera.GetPath());
        player.Connect("Died", new Callable(this, "ShowDeathScreen"));
        gui.SetPlayer(player);
    }

    public void ShowDeathScreen()
    {
        gui.SetPlayer(null);
        DeathScreen deathScreen = deathScreenScene.Instantiate() as DeathScreen;
        deathScreen.Connect("RespawnPlayer", new Callable(this, "SpawnPlayer"));
        AddChild(deathScreen);
    }

    private void HandlePlayerVictory()
    {
        GameOverScreen gameOver = gameOverScene.Instantiate() as GameOverScreen;
        AddChild(gameOver);
        gameOver.SetTitle(true);
        GetTree().Paused = true;
    }

    private void HandlePlayerDefeat()
    {
        GameOverScreen gameOver = gameOverScene.Instantiate() as GameOverScreen;
        AddChild(gameOver);
        gameOver.SetTitle(false);
        GetTree().Paused = true;
    }

    public void Pause()
    {
        PauseScreen pauseScreen = pauseMenuScene.Instantiate() as PauseScreen;
        AddChild(pauseScreen);
    }

    private void SetCameraLimits()
    {
        Rect2 mapLimits = ground.GetUsedRect();
        Vector2 mapCellSize = ground.TileSet.TileSize;
        camera.LimitLeft = (int)(mapLimits.Position.X * mapCellSize.X);
        camera.LimitRight = (int)(mapLimits.End.X * mapCellSize.X);
        camera.LimitTop = (int)(mapLimits.Position.Y * mapCellSize.Y);
        camera.LimitBottom = (int)(mapLimits.End.Y * mapCellSize.Y);
    }
}
