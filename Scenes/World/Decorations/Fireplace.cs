using DuchyofThorns.Scenes.Globals;

namespace DuchyOfThorns;

/// <summary>
/// Class for fireplace functionality
/// </summary>
public partial class Fireplace : StaticBody2D
{
    protected Globals globals;
    [Export] PackedScene fireArrow;
    AudioStreamPlayer2D firePlayer;
    AudioStreamPlayer2D setOnFirePlayer;
    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>("/root/Globals");
        firePlayer = GetNode<AudioStreamPlayer2D>("FirePlayer");
        setOnFirePlayer = GetNode<AudioStreamPlayer2D>("SetOnFirePlayer");
    }
    public void SetOnFire(Team team, Vector2 position, Vector2 direction)
    {
        setOnFirePlayer.Play();
        FireArrow arrow = fireArrow.Instantiate() as FireArrow;
        arrow.Damage = arrow.Damage * 2;
        globals.EmitSignal(nameof(Globals.ArrowFired), arrow, (int)team, position, direction);
    }
}
