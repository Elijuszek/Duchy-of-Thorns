using Godot.Collections;

namespace DuchyOfThorns;

/// <summary>
/// TODO: Rework capturableBaseSystem
/// MapAI class which handles the spawning of units and the capturing of bases
/// </summary>
public partial class MapAI : Node2D
{
	[Export] protected BaseCaptureOrder baseCaptureOrder = BaseCaptureOrder.FIRST;
	[Export] protected Team team = Team.NEUTRAL;
	[Export] protected Array<Marker2D> respawnPoints;
	[Export] protected CapturableBaseManager capturableBaseManager;

	protected CapturableBase targetBase = null;
	protected Troop[] aliveTroops;
    public override void _Ready()
    {
        base._Ready();
		// Get respawns

		foreach(CapturableBase cBase in capturableBaseManager.GetCapturableBases())
		{
			cBase.Connect("BaseCaptured", new Callable(this, "HandleBaseCaptured"));
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


	protected void AssignNextCapturableBase(CapturableBase cBase)
	{
		foreach (Troop troop in aliveTroops)
		{
			troop.Destination = cBase.GetRandomPositionWithinRadius();
		}
	}
}
