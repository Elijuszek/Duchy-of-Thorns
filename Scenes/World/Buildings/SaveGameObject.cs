using Godot;

public partial class SaveGameObject : StaticBody2D
{
    private Globals globals;
    private Control saveGame;
    private Label saveLabel;
    public override void _Ready()
    {
        saveGame = GetNode<Control>("SaveGame");
        globals = GetNode<Globals>("/root/Globals");
        saveLabel = saveGame.GetNode<Label>("PanelContainer/MarginContainer/Rows/Label");
    }
    private void Area2DBodyEntered(Player player)
    {
        saveGame.Show();
    }
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
