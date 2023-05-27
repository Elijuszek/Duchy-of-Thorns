namespace DuchyOfThorns;
public partial class MapAI : Node2D
{
	private enum BaseCaptureOrder
	{
		FIRST,
		LAST
	}
	[Export] private BaseCaptureOrder baseCaptureOrder;
	[Export] protected Team.TeamName teamName = Team.TeamName.NEUTRAL;

	protected CapturableBase targetBase = null;
	protected CapturableBase[] capturableBases;
	protected Respawn[] respawnPoints;
	protected int NextSpawn = 0;
	protected Pathfinding pathfinding;

	protected Team team;

	public override void _Ready()
	{
		team = GetNode<Team>("Team");
	}
	public virtual void Initialize(CapturableBase[] capturableBases, Respawn[] respawnPoints, Pathfinding pathfinding)
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
		}
		CheckForNextCapturableBases();
		foreach (Respawn respawn in respawnPoints)
		{
			respawn.SpawnUnit();
		}
	}
	protected void HandleBaseCaptured(int newTeam)
	{
		CheckForNextCapturableBases();
	}
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
			for (int i = listOfBases - 1; i > 0; i--)
			{
				CapturableBase cBase = capturableBases[i];
				if (team.team != cBase.Team.team)
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
				if (team.team != cBase.Team.team)
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
			respawn.SetCapturableBase(cBase, cBase.GetRandomPositionWithinRadius());
		}
	}
}
