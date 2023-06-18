using DuchyofThorns.Scenes.Globals;

namespace DuchyOfThorns;

/// <summary>
/// Class for managing capturable bases
/// </summary>
public partial class CapturableBaseManager : Node2D
{
    [Signal] public delegate void PlayerCapturedAllBasesEventHandler();
    [Signal] public delegate void PlayerLostAllBasesEventHandler();
    private CapturableBase[] capturableBases;
    public override void _Ready()
    {
        capturableBases = GetChildren().OfType<CapturableBase>().ToArray();
        foreach (CapturableBase cBase in capturableBases)
        {
            cBase.Connect("BaseCaptured", new Callable(this, "HandleBaseCaptured"));
        }
    }
    public CapturableBase[] GetCapturableBases()
    {
        return capturableBases;
    }
    public void SetTeam(Team team)
    {
        foreach (CapturableBase b in capturableBases)
        {
            b.SetTeam(team);
        }
    }
    private void HandleBaseCaptured(int team)  // TODO Team might be not needed singal from capturablebase connected with through mapAI
    {
        int playerBases = 0;
        int enemyBases = 0;
        int totalBases = capturableBases.Length;
        foreach (CapturableBase cBase in capturableBases)
        {
            switch (cBase.Team)
            {
                case Team.PLAYER:
                    playerBases++;
                    break;
                case Team.ENEMY:
                    enemyBases++;
                    break;
                case Team.NEUTRAL:
                    return;
            }
        }
        if (playerBases == totalBases)
        {
            EmitSignal(nameof(PlayerCapturedAllBases));
        }
        else if (enemyBases == totalBases)
        {
            EmitSignal(nameof(PlayerLostAllBases));
        }
    }

}
