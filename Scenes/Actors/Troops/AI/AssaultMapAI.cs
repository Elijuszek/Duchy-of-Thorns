using DuchyofThorns;
using Godot.Collections;
namespace DuchyOfThorns;

/// <summary>
/// MapAI which is dedicated for defend maps, acts as a wave manager
/// </summary>
public partial class AssaultMapAI : MapAI
{
	[Signal] public delegate void PlayerVictoryEventHandler(int reward);

	[Export] private Array<WaveInfo> waves;

	[Export] private CapturableBaseManager capturableBaseManager;

	private Marker2D[] respawnPoints;
	public int CurrentWave { get; set; } = 0;
	private Timer waveTimer;
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

	//public override void Initialize(CapturableBase[] capturableBases, Respawn[] respawnPoints)
	//{
	//	if (capturableBases.Length == 0 || respawnPoints.Length == 0)
	//	{
	//		GD.PushError("ASSAULT MAPAI IS NOT PROPERLY INITIALIZED!");
	//		return;
	//	}
	//	this.respawnPoints = respawnPoints;
	//	this.capturableBases = capturableBases;
	//	foreach (CapturableBase cBase in capturableBases)
	//	{
	//		cBase.Connect("BaseCaptured", new Callable(this, "HandleBaseCaptured"));
	//	}
	//	foreach (Respawn respawn in respawnPoints)
	//	{
	//		respawn.Connect("OutOfTroops", new Callable(this, "HandleOutOfTroops"));
	//	}



	//	CheckForNextCapturableBases();
	//}

	public void SpawnNextWave()
	{
		Wave current = waves[CurrentWave];
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

	public Dictionary<string, Variant> Save()
	{
		return new Dictionary<string, Variant>()
		{
			{ "Filename", SceneFilePath },
			{ "Parent", GetParent().GetPath() },
			{ "PosX", Position.X }, // Vector2 is not supported by JSON
			{ "PosY", Position.Y },
			{ "CurrentWave", CurrentWave },
		};
	}

	public void Load(Dictionary<string, Variant> data) => CurrentWave = (int)data["CurrentWave"];
}
