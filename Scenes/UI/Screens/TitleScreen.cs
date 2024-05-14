namespace DuchyOfThorns;

/// <summary>
/// Class for title screen functionality
/// </summary>
public partial class TitleScreen : Control
{
    [Export] private AnimationPlayer animationPlayer;
    [Export] private AudioStreamPlayer click;
    [Export] private PackedScene settingsScene;
    private SettingsScreen settingsScreen;

    private Globals globals;
    private readonly Random randomizer = new();

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");

        settingsScreen = settingsScene.Instantiate<SettingsScreen>();
        settingsScreen.Visible = false;
        AddChild(settingsScreen);

        RandomizeBackground();
    }

    private void RandomizeBackground()
    {
        animationPlayer.Play("Background", customSpeed: 0.0f);
        animationPlayer.Seek(randomizer.NextDouble() * animationPlayer.CurrentAnimationLength, false);
    }
    public void NewGameButtonPressed()
    {
        // Click sound
        click.Play();
        globals.loadingForm = LoadingForm.New;

        // Changing scene
        GetTree().ChangeSceneToFile("res://Scenes/World/Main/DefendWorld/DefendWorld.tscn");
    }
    public void SavedGamesButtonPressed()
    {
        click.Play();
        globals.loadingForm = LoadingForm.Save;

        // Changing scene through globals
        globals.LoadGame();
        QueueFree();
    }
    public void SettingsButtonPressed()
    {
        click.Play();

        settingsScreen.Visible = true;
    }
    public void QuitGameButtonPressed()
    {
        click.Play();
        GetTree().Quit();
    }
}
