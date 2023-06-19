namespace DuchyOfThorns;

/// <summary>
/// Class for pause screen functionality
/// </summary>
public partial class PauseScreen : CanvasLayer
{
    public override void _Ready() => GetTree().Paused = true;
    private void ResumeButtonPressed()
    {
        GetTree().Paused = false;
        QueueFree();
    }
    private void LoadGameButtonPressed()
    {
        Globals globals = GetNode<Globals>("/root/Globals");
        globals.loadingForm = LoadingForm.Save;
        globals.LoadGame();
        GetTree().Paused = false;
        QueueFree();
    }
    private void SettingsButtonPressed()
    {
        return;
    }
    private void ExitToMainMenuButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/UI/TitleScreen.tscn");
    }
}
