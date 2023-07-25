namespace DuchyOfThorns;

/// <summary>
/// Base class for all maps
/// </summary>
public partial class World : Node2D
{
    [Export] protected TileMap tileMap;
    [Export] protected Marker2D playerSpawn;
    [Export] protected Camera2D camera;
    [Export] protected ProjectileManager projectileManager;
    [Export] protected LootManager lootManager;
    [Export] protected AudioStreamPlayer backgroundMusic;
    [Export] protected GUI gui;

    protected Player player;
    protected PackedScene gameOverScene;
    protected PackedScene playerScene;
    protected PackedScene pauseMenuScene;
    protected PackedScene deathScreenScene;
    protected Globals globals;

    public override void _Ready()
    {
        base._Ready();
        gameOverScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/GUI/GameOverScreen.tscn");
        playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Characters/Player/Player.tscn");
        pauseMenuScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/GUI/PauseScreen.tscn");
        deathScreenScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/GUI/DeathScreen.tscn");

        globals = GetNode<Globals>("/root/Globals");

        globals.Connect("ProjectileFired", new Callable(projectileManager, "HandleProjectileFired"));
        globals.Connect("CoinsDroped", new Callable(lootManager, "HandleCoinsSpawned"));

        switch (globals.loadingForm)
        {
            case LoadingForm.Load:
                LoadPlayer();
                break;
            case LoadingForm.Save:
                break;
            case LoadingForm.New:
                SpawnPlayer();
                break;
        }
        SetCameraLimits();
        //backgroundMusic.Play();
        GetTree().CurrentScene = this;
    }

    protected void SpawnPlayer()
    {
        player = playerScene.Instantiate<Player>();
        player.Position = playerSpawn.Position;
        AddChild(player);
        player.SetCameraTransform(camera.GetPath());
        player.Connect("Died", new Callable(this, "ShowDeathScreen"));
        gui.SetPlayer(player);
        globals.Player = player.Save();
    }

    protected void LoadSavedPlayer(Godot.Collections.Dictionary<string, Variant> save)
    {
        player = playerScene.Instantiate<Player>();
        player.Position = playerSpawn.Position;
        AddChild(player);
        player.Load(save);
        player.SetCameraTransform(camera.GetPath());
        player.Connect("Died", new Callable(this, "ShowDeathScreen"));
        gui.SetPlayer(player);
        globals.Player = player.Save();
    }

    protected void LoadPlayer()
    {
        player = playerScene.Instantiate<Player>();
        player.Position = playerSpawn.Position;
        AddChild(player);
        player.SetCameraTransform(camera.GetPath());
        player.Connect("Died", new Callable(this, "ShowDeathScreen"));
        gui.SetPlayer(player);
        player.Load(globals.Player);
    }

    public void ShowDeathScreen()
    {
        gui.SetPlayer(null);
        DeathScreen deathScreen = deathScreenScene.Instantiate<DeathScreen>();
        deathScreen.Connect("RespawnPlayer", new Callable(this, "SpawnPlayer"));
        AddChild(deathScreen);
    }

    public void Pause()
    {
        PauseScreen pauseScreen = pauseMenuScene.Instantiate<PauseScreen>();
        AddChild(pauseScreen);
    }

    protected void SetCameraLimits()
    {
        Rect2 mapLimits = tileMap.GetUsedRect();
        Vector2 mapCellSize = tileMap.TileSet.TileSize;
        camera.LimitLeft = (int)(mapLimits.Position.X * mapCellSize.X);
        camera.LimitRight = (int)(mapLimits.End.X * mapCellSize.X);
        camera.LimitTop = (int)(mapLimits.Position.Y * mapCellSize.Y);
        camera.LimitBottom = (int)(mapLimits.End.Y * mapCellSize.Y);
    }

    public virtual Godot.Collections.Dictionary<string, Variant> Save()
    {
        return new Godot.Collections.Dictionary<string, Variant>()
        {
            { "Filename", SceneFilePath },
            { "Parent", GetParent().GetPath() },
            { "PosX", Position.X }, // Vector2 is not supported by JSON
			{ "PosY", Position.Y },
            { "Player", GetNode<Player>("Player").Save() }
        };
    }

    public virtual void Load(Godot.Collections.Dictionary<string, Variant> save)
    {
        if (globals.loadingForm == LoadingForm.Save)
        {
            LoadSavedPlayer(new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)save["Player"]));
            player = GetNode<Player>("Player");
        }
    }
}
