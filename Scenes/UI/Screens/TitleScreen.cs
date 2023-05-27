namespace DuchyOfThorns;

public partial class TitleScreen : Control
{
    private AnimationPlayer animationPlayer;
    private Globals globals;
    private AudioStreamPlayer click;
    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>("/root/Globals");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        click = GetNode<AudioStreamPlayer>("Click");
        animationPlayer.Play("Background");
        animationPlayer.Seek(GetRandomTime(), true);
        animationPlayer.Stop();
    }
    // For getting random background
    private float GetRandomTime()
    {
        Random rand = new Random();
        double min = 0;
        double max = animationPlayer.CurrentAnimationLength;
        double range = max - min;
        double sample = rand.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }
    public void NewGameButtonPressed()
    {
        // Click sound
        click.Play();
        globals.loadingForm = Globals.LoadingForm.New;

        // Changing scene
        GetTree().ChangeSceneToFile("res://Scenes/World/Main/DefendMap/DefendMap.tscn");
    }
    public void SavedGamesButtonPressed()
    {
        click.Play();
        globals.loadingForm = Globals.LoadingForm.Save;

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
