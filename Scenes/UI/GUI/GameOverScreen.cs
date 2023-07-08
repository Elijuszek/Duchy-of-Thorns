namespace DuchyOfThorns;

/// <summary>
/// Class for game over screen functionality
/// </summary>
public partial class GameOverScreen : CanvasLayer
{
    [Export] private Label title;
    public Player player { get; set; }
    private Globals globals;
    public override void _Ready()
    {
        title = GetNode<Label>("PanelContainer/MarginContainer/Rows/CenterContainer/VBoxContainer/Title");
        globals = GetNode<Globals>("/root/Globals");
    }
    public void SetTitle(bool win)
    {
        if (win)
        {
            title.Text = "Victory!";
            title.Modulate = Color.Color8(6, 125, 26);
        }
        else
        {
            title.Text = "Defeat!";
            title.Modulate = Color.Color8(158, 9, 22);
        }
    }
    private void RestartButtonPressed()
    {
        GetTree().Paused = false;
        globals.ChangeScenes(player, "res://Scenes/World3D/Maps/Cities/Rosethorn.tscn", 0.5f);
    }
    private void LoadGameButtonPressed()
    {
        globals.loadingForm = LoadingForm.Save;
        globals.LoadGame();
        QueueFree();
    }
    private void MainMenuButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/UI/TitleScreen.tscn");
    }
    private void QuitButtonPressed() => GetTree().Quit();
}
