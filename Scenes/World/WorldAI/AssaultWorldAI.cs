using Godot.Collections;

namespace DuchyOfThorns;

/// <summary>
/// MapAI which is dedicated for defend maps, acts as a wave manager
/// </summary>
public partial class AssaultWorldAI : WorldAI
{
	[Signal] public delegate void PlayerVictoryEventHandler(int reward);

	[Export] private Array<WaveInfo> waves;
    [Export] private Timer waveTimer;
	[Export] private AudioStreamPlayer warHorn;
    [Export] protected CapturableBaseManager capturableBaseManager;

    protected CapturableBase targetBase = null;
    public int CurrentWave { get; set; } = 0;

	private int aliveRespawns;
	private AudioStreamOggVorbis[] hornSounds =
	{
			ResourceLoader.Load<AudioStreamOggVorbis>("res://Sounds/Horns/DistantWarhorn1.ogg"),
			ResourceLoader.Load<AudioStreamOggVorbis>("res://Sounds/Horns/DistantWarhorn2.ogg"),
	};

	public override void _Ready()
	{
		base._Ready();
		random = new Random();
	}

	public void SpawnNextWave()
	{
		WaveInfo current = waves[CurrentWave];
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
		//foreach (Respawn respawn in respawnPoints)
		//{
		//	respawn.Clear();
		//}
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

    protected void HandleBaseCaptured(int newTeam) => CheckForNextCapturableBases();

    protected void CheckForNextCapturableBases()
    {
        //CapturableBase nextBase = GetNextCapturableBase();
        //if (nextBase != null)
        //{
        //	targetBase = nextBase;
        //	AssignNextCapturableBase(nextBase);
        //}
    }

    protected void AssignNextCapturableBase(CapturableBase cBase)
    {
        //foreach (Troop troop in aliveTroops)
        //{
        //    troop.Destination = cBase.GetRandomPositionWithinRadius();
        //}
    }
    public void Load(Dictionary<string, Variant> data) => CurrentWave = (int)data["CurrentWave"];
}
