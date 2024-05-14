namespace DuchyOfThorns;

/// <summary>
/// Class for pause screen functionality
/// </summary>
public partial class PauseScreen : CanvasLayer
{
    [Export] private PackedScene settingsScene;
    private SettingsScreen settingsScreen;

    public override void _Ready()
    {
        GetTree().Paused = true;

        settingsScreen = settingsScene.Instantiate<SettingsScreen>();
        settingsScreen.Visible = false;
        AddChild(settingsScreen);
    }
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
        settingsScreen.Visible = true;
    }
    private void ExitToMainMenuButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/UI/Screens/TitleScreen.tscn");
    }
}
