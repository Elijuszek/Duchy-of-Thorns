namespace DuchyOfThorns;

/// <summary>
/// Class for showing save game label
/// </summary>
public partial class SaveGameObject : StaticBody2D
{
    [Export] private Control saveGame;
    [Export] private Label saveLabel;

    private Globals globals;
    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");
    }
    private void Area2DBodyEntered(Player player) => saveGame.Show();
    private void Area2DBodyExited(Player player)
    {
        saveLabel.Hide();
        saveGame.Hide();
    }
    private void SaveGameButtonPressed()
    {
        if (globals.SaveGame())
        {
            saveLabel.Show();
        };
    }
}
