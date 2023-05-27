namespace DuchyOfThorns;

/// <summary>
/// Final class for the guard map
/// </summary>
public partial class Rosethorn : GuardMap
{
    public override void _Ready() => base._Ready();
    private void BottomExitBodyEntered(Player body)
    {
        globals.ChangeScenes(body, "res://Scenes/World/Maps/CaptureFields/Westwend.tscn", 0.5f);
    }
    private void BottomExitBodyExited(Player body) => globals.StopChangingScenes();
}