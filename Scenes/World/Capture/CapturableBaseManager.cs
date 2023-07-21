using Godot.Collections;

namespace DuchyOfThorns;

/// <summary>
/// Class for managing capturable bases
/// </summary>
public partial class CapturableBaseManager : Node2D
{
    [Signal] public delegate void PlayerCapturedAllBasesEventHandler();
    [Signal] public delegate void PlayerLostAllBasesEventHandler();
    public Array<CapturableBase> capturableBases { get; private set; }
    public override void _Ready()
    {
        if (capturableBases.Count <= 0)
        {
            GD.PushError("CAPTURABLE BASE MANAGER IS NOT PROPERLY INITIALIZED!\n MISSING CAPTURABLE BASES.");
            return;
        }
        foreach (CapturableBase cBase in capturableBases)
        {
            cBase.Connect("BaseCaptured", new Callable(this, "HandleBaseCaptured"));
        }
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
        int totalBases = capturableBases.Count;
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

    /// <summary>
    /// Method for getting the next capturable base, 
    /// if all bases are captured, returns last captured base in the array according to the capture order
    /// </summary>
    /// <param name="team">Attacker team</param>
    /// <param name="captureOrder">order in which bases are captured</param>
    /// <returns>Capturable base object</returns>
    public CapturableBase GetNextCapturableBase(Team team, BaseCaptureOrder captureOrder)
    {
        if (captureOrder == BaseCaptureOrder.LAST)
        {
            for (int i = capturableBases.Count - 1; i >= 0; i--)
            {
                CapturableBase cBase = capturableBases[i];
                if (team != cBase.Team)
                {
                    return cBase;
                }
            }
            return capturableBases[0];
        }
        for (int i = 0; i < capturableBases.Count; i++)
        {
            CapturableBase cBase = capturableBases[i];
            if (team != cBase.Team)
            {
                return cBase;
            }
        }
        return capturableBases[capturableBases.Count - 1];
    }

}
