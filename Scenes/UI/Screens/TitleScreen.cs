namespace DuchyOfThorns;

/// <summary>
/// Class for title screen functionality
/// </summary>
public partial class TitleScreen : Control
{
    [Export] private AnimationPlayer animationPlayer;
    [Export] private AudioStreamPlayer click;

    private Globals globals;

    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>("/root/Globals");
        animationPlayer.Play("Background", customSpeed: 0.0f);
        animationPlayer.Seek(GetRandomTime(), false);
    }
    // For getting random background
    private double GetRandomTime()
    {
        Random rand = new Random();
        double min = 0;
        double max = animationPlayer.CurrentAnimationLength;
        double range = max - min;
        double sample = rand.NextDouble();
        double scaled = (sample * range) + min;
        return scaled;
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
        return;
    }
    public void QuitGameButtonPressed()
    {
        click.Play();
        GetTree().Quit();
    }
}
