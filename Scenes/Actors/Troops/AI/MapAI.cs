namespace DuchyOfThorns;

/// <summary>
/// TODO: Rework capturableBaseSystem
/// MapAI class which handles the spawning of units and the capturing of bases
/// </summary>
public partial class MapAI : Node2D
{
	[Export] private BaseCaptureOrder baseCaptureOrder = BaseCaptureOrder.FIRST;
	[Export] protected Team team = Team.NEUTRAL;

	protected CapturableBase targetBase = null;
	protected CapturableBase[] capturableBases;
	protected Respawn[] respawnPoints;
	protected int NextSpawn = 0;

    public virtual void Initialize(CapturableBase[] capturableBases, Respawn[] respawnPoints)
	{
		if (capturableBases.Length == 0)
		{
			GD.PushError("MAPAI IS NOT PROPERLY INITIALIZED!");
			return;
		}
		this.respawnPoints = respawnPoints;
		this.capturableBases = capturableBases;
		foreach (CapturableBase cBase in capturableBases)
		{
			cBase.Connect("BaseCaptured", new Callable(this, "HandleBaseCaptured"));
		}
		CheckForNextCapturableBases();
		foreach (Respawn respawn in respawnPoints)
		{
			respawn.SpawnUnit();
		}
	}
	protected void HandleBaseCaptured(int newTeam) => CheckForNextCapturableBases();
    protected void CheckForNextCapturableBases()
	{
		CapturableBase nextBase = GetNextCapturableBase();
		if (nextBase != null)
		{
			targetBase = nextBase;
			AssignNextCapturableBase(nextBase);
		}
	}
	protected CapturableBase GetNextCapturableBase()
	{
        int listOfBases = capturableBases.Length;
        if (baseCaptureOrder == BaseCaptureOrder.LAST)
		{
			for (int i = listOfBases - 1; i >= 0; i--)
			{
                CapturableBase cBase = capturableBases[i];
				if (team != cBase.Team)
				{
                    return cBase;
				}
			}
		}
		else
		{
			for (int i = 0; i < listOfBases; i++)
			{
				CapturableBase cBase = capturableBases[i];
				if (team != cBase.Team)
				{
					return cBase;
				}
			}
		}
		return null;
	}
	protected void AssignNextCapturableBase(CapturableBase cBase)
	{
		foreach (Respawn respawn in respawnPoints)
		{
			respawn.SetCapturableBase(cBase.GetRandomPositionWithinRadius());
		}
	}
}
