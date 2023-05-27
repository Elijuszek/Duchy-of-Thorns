namespace DuchyOfThorns;

/// <summary>
/// MapAI which is dedicated for defend maps, acts as a wave manager
/// </summary>
public partial class AssaultMapAI : MapAI
{
	[Signal] public delegate void PlayerVictoryEventHandler(int reward);
	public int CurrentWave { get; set; } = 0;
	private Timer waveTimer;
	private Wave[] waves;
	private InfantryRespawn[] infantryRespawns;
	private RangedRespawn[] rangedRespawns;
	private CavalryRespawn[] cavalryRespawns;
	private int aliveRespawns;
	private AudioStreamPlayer warHorn;
	private AudioStreamOggVorbis[] hornSounds =
	{
			ResourceLoader.Load<AudioStreamOggVorbis>("res://Sounds/Horns/DistantWarhorn1.ogg"),
			ResourceLoader.Load<AudioStreamOggVorbis>("res://Sounds/Horns/DistantWarhorn2.ogg"),
	};
	private Random random;
	public override void _Ready()
	{
		base._Ready();
		waveTimer = GetNode<Timer>("ElapsedWaveTime");
		waves = GetNode<Node>("Waves").GetChildren().OfType<Wave>().ToArray();
		warHorn = GetNode<AudioStreamPlayer>("WarHorn");
		random = new Random();
	}
	public override void Initialize(CapturableBase[] capturableBases, Respawn[] respawnPoints, Pathfinding pathfinding)
	{
		if (capturableBases.Length == 0 || respawnPoints.Length == 0)
		{
			GD.PushError("MAP RangedAI IS NOT PROPERLY INITIALIZED!");
			return;
		}
		team.team = teamName;
		this.pathfinding = pathfinding;
		this.respawnPoints = respawnPoints;
		this.capturableBases = capturableBases;
		foreach (CapturableBase cBase in capturableBases)
		{
			cBase.Connect("BaseCaptured", new Callable(this, "HandleBaseCaptured"));
		}
		foreach (Respawn respawn in respawnPoints)
		{
			respawn.Initialize(pathfinding);
			respawn.Connect("OutOfTroops", new Callable(this, "HandleOutOfTroops"));
		}
		infantryRespawns = respawnPoints.OfType<InfantryRespawn>().ToArray();
		rangedRespawns = respawnPoints.OfType<RangedRespawn>().ToArray();
		cavalryRespawns = respawnPoints.OfType<CavalryRespawn>().ToArray();
		CheckForNextCapturableBases();
	}
	public void SpawnNextWave()
	{
		Wave current = waves[CurrentWave];
		aliveRespawns = infantryRespawns.Length + rangedRespawns.Length + cavalryRespawns.Length;
		waveTimer.WaitTime = current.Duration;
		for (int i = 0; i < infantryRespawns.Length; i++)
		{
			infantryRespawns[i].Unit = current.GetInfantryUnits(i);
			infantryRespawns[i].RespawnCount = current.GetInfantryRespawnCount(i);
			infantryRespawns[i].RespawnCooldown = current.GetInfantryCooldown(i);
			infantryRespawns[i].SpawnUnit();
		}
		for (int i = 0; i < rangedRespawns.Length; i++)
		{
			rangedRespawns[i].Unit = current.GetRangedUnits(i);
			rangedRespawns[i].RespawnCount = current.GetRangedRespawnCount(i);
			rangedRespawns[i].RespawnCooldown = current.GetRangedCooldown(i);
			rangedRespawns[i].SpawnUnit();
		}
		for (int i = 0; i < cavalryRespawns.Length; i++)
		{
			cavalryRespawns[i].Unit = current.GetCavalryUnits(i);
			cavalryRespawns[i].RespawnCount = current.GetCavalryRespawnCount(i);
			cavalryRespawns[i].RespawnCooldown = current.GetCavalryCooldown(i);
			cavalryRespawns[i].SpawnUnit();
		}
		waveTimer.Start();
		warHorn.Stream = hornSounds[random.Next(0, 1)];
		warHorn.Play();
	}
	private void HandleVicotry()
	{
		ClearMap();
		EmitSignal(nameof(PlayerVictory), waves[CurrentWave].Reward);
		CurrentWave++;
	}
	public void ClearMap()
	{
		waveTimer.Stop();
		foreach (Respawn respawn in respawnPoints)
		{
			respawn.Clear();
		}
	}
	private void HandleOutOfTroops()
	{
		aliveRespawns--;
		if (aliveRespawns > 0 || waveTimer.IsStopped())
		{
			return;
		}
		HandleVicotry();
	}
	private void ElapsedWaveTimeTimeout()
	{
		ClearMap();
	}
	public Godot.Collections.Dictionary<string, Variant> Save()
	{
		return new Godot.Collections.Dictionary<string, Variant>()
		{
			{ "Filename", SceneFilePath },
			{ "Parent", GetParent().GetPath() },
			{ "PosX", Position.X }, // Vector2 is not supported by JSON
			{ "PosY", Position.Y },
			{ "CurrentWave", CurrentWave },
		};
	}
	public void Load(Godot.Collections.Dictionary<string, Variant> data)
	{
		CurrentWave = (int)data["CurrentWave"];
	}
}
