using Godot.Collections;
using System.Reflection.Metadata;

namespace DuchyOfThorns;

/// <summary>
/// WorldAI which is dedicated for defend maps, acts as a wave manager
/// </summary>
public partial class AssaultWorldAI : Node2D
{
	[Signal] public delegate void PlayerVictoryEventHandler(int reward);

    // Export is used only for testing purposes
    [Export] public int CurrentWaveIndex { get; set; } = 0;
    [Export] protected CapturableBaseManager capturableBaseManager;
    [Export] private Array<WaveInfo> waves;
    [Export] private Timer waveTimer;
    [Export] private AudioStreamPlayer warHorn;
	[Export] private TroopsManager troopsManager;
    [Export] private AudioStreamOggVorbis[] hornSounds;
    public CapturableBase TargetBase { get; set; }

	private WaveInfo currentWave;
	private Random random;

	private int troopsInScene = 0;

	public override void _Ready()
	{
		base._Ready();
		random = new Random();
	}

	public void SpawnNextWave()
	{
        // TODO: currently fixed waves count
        currentWave = waves[CurrentWaveIndex].Duplicate();
		waveTimer.Start();
		warHorn.Stream = hornSounds[random.Next(0, 1)];
		warHorn.Play();

		for(int i = 0; i < currentWave.MaxUnits; i++)
		{
			SpawnUnit();
		}
    }
	
	private void SpawnUnit()
	{
		TroopType type = currentWave.DequeuUnit();

		if (type == TroopType.NONE)
		{
			if (troopsInScene == 0)
			{
				HandleVicotry();
			}
			return;
		}

		CapturableBase cbase = capturableBaseManager.GetNextCapturableBase(Team.ENEMY, BaseCaptureOrder.FIRST);
		// TODO: select spawn points
		Troop spawnedTroop = troopsManager.HandleTroopSpawned(type, currentWave.UnitQueue[0].Stats,
			new Vector2(Utilities.GetRandomFloat(200f, 400f), Utilities.GetRandomFloat(200f, 400f)),
			cbase.GetDestination());

        spawnedTroop.RemovedFromScene += HandleTroopRemoved;
		troopsInScene++;
	}

	private void HandleTroopRemoved(IPoolable source)
	{
		troopsInScene--;
        SpawnUnit();
    }

	private void HandleVicotry()
	{
		ClearWorld();
		EmitSignal(nameof(PlayerVictory), currentWave.Reward);
		CurrentWaveIndex++;
	}

	public void ClearWorld()
	{
        foreach (Troop troop in troopsManager.GetChildren().OfType<Troop>())
        {
			troop.RemoveFromScene();
        }
        waveTimer.Stop();
	}

	private void ElapsedWaveTimeTimeout()
	{
		ClearWorld();
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

    public Dictionary<string, Variant> Save()
    {
        return new Dictionary<string, Variant>()
        {
            { "Filename", SceneFilePath },
            { "Parent", GetParent().GetPath() },
            { "PosX", Position.X }, // Vector2 is not supported by JSON
			{ "PosY", Position.Y },
            { "CurrentWave", CurrentWaveIndex },
        };
    }

    public void Load(Dictionary<string, Variant> data) => CurrentWaveIndex = (int)data["CurrentWave"];
}
