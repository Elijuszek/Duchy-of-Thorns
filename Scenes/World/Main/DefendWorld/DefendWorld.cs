using Godot.Collections;

namespace DuchyOfThorns;

/// <summary>
/// Class of the map where player has to defend his bases from enemy attacks
/// </summary>
public partial class DefendWorld : World
{
    [Export] private AssaultWorldAI assaultworldAI;
    [Export] private WorldAI allyWorldAI;
    [Export] private CapturableBaseManager capturableBaseManager;

    private PackedScene assaultOverScreen;
    private int safeGold = 0;
    public override void _Ready()
    {
        base._Ready();
        assaultOverScreen = ResourceLoader.Load<PackedScene>("res://Scenes/UI/GUI/AssaultOverScreen.tscn");
        //CapturableBase[] bases = capturableBaseManager.GetCapturableBases();

        Respawn[] allyRespawnPoints = GetNode<Node2D>("AllyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();
        Respawn[] enemyRespawnPoints = GetNode<Node2D>("EnemyRespawnPoints").GetChildren().OfType<Respawn>().ToArray();

        // TODO Signal is still emitted
        //capturableBaseManager.Connect("PlayerCapturedAllBases", new Callable(this, "HandlePlayerVictory"));

        assaultworldAI.Connect("PlayerVictory", new Callable(this, "HandlePlayerVictory"));
        capturableBaseManager.Connect("PlayerLostAllBases", new Callable(this, "HandlePlayerDefeat"));
        capturableBaseManager.SetTeam(Team.PLAYER);

        gui.Connect("NewWaveStarted", new Callable(this, "NewWave"));
        gui.ToggleNewWaveButton(true);

    }
    private void HandlePlayerVictory(int reward)
    {
        int loot = 0;
        Player player = GetNodeOrNull<Player>("Player");
        if (player != null)
        {
            loot = player.Stats.Gold;
        }
        safeGold += loot + reward;
        AssaultOverScreen assaultOver = assaultOverScreen.Instantiate<AssaultOverScreen>();
        assaultOver.Connect("Continue", new Callable(this, "UpgradePhase"));
        AddChild(assaultOver);
        assaultOver.Initialize(true, loot, reward, safeGold);
    }
    private void HandlePlayerDefeat()
    {
        assaultworldAI.ClearWorld();
        int loot = 0;
        Player player = GetNodeOrNull<Player>("Player");
        if (player != null)
        {
            loot = player.Stats.Gold;
            safeGold += loot;
        }
        AssaultOverScreen assaultOver = assaultOverScreen.Instantiate<AssaultOverScreen>();
        assaultOver.Connect("Continue", new Callable(this, "UpgradePhase"));
        AddChild(assaultOver);
        assaultOver.Initialize(false, loot, 0, safeGold);
    }
    private void UpgradePhase()
    {
        if (!HasNode("Player"))
        {
            if (HasNode("DeathScreen"))
            {
                GetNode<DeathScreen>("DeathScreen").QueueFree();
            }
            LoadPlayer(); // TODO might need a change
        }
        Player player = GetNode<Player>("Player");
        player.Stats.Gold = 0;
        player.GetGold(safeGold);
        capturableBaseManager.SetTeam(Team.PLAYER);
        globals.SaveGame();
        gui.ToggleNewWaveButton(true);
    }
    private void NewWave()
    {
        safeGold = player.Stats.Gold;
        player.Stats.Gold = 0;
        player.SetGold(0);

        // TODO: New wave should load after the last wave is finished
        assaultworldAI.SpawnNextWave();
    }
    public override Dictionary<string, Variant> Save()
    {
        Dictionary<string, Variant> save = base.Save();
        save.Add("AssaultMapAI", assaultworldAI.Save());
        return save;
    }
    public override void Load(Dictionary<string, Variant> save)
    {
        base.Load(save);
        assaultworldAI.Load(new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)save["AssaultMapAI"]));
    }

}
