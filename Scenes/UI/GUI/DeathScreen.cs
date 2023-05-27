namespace DuchyOfThorns;

public partial class DeathScreen : CanvasLayer
{
    [Signal] public delegate void RespawnPlayerEventHandler();
    Globals globals;
    private void RespawnButtonPressed()
    {
        EmitSignal(nameof(RespawnPlayer));
        QueueFree();
    }
    private void LoadGameButtonPressed()
    {
        globals = GetNode<Globals>("/root/Globals");
        globals.loadingForm = Globals.LoadingForm.Save;
        globals.LoadGame();
        QueueFree();
    }
    private void ExitToMainMenuButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/UI/TitleScreen.tscn");
    }
}
