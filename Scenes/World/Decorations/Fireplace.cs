namespace DuchyOfThorns;

/// <summary>
/// Class for fireplace functionality
/// </summary>
public partial class Fireplace : StaticBody2D
{
    [Export] PackedScene fireArrow;
    [Export] AudioStreamPlayer2D firePlayer;
    [Export] AudioStreamPlayer2D setOnFirePlayer;

    protected Globals globals;

    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>("/root/Globals");
    }

    public void SetOnFire(Team team, Vector2 position, Vector2 direction)
    {
        setOnFirePlayer.Play();
        FireArrow arrow = fireArrow.Instantiate() as FireArrow;
        arrow.Damage = arrow.Damage * 2;
        globals.EmitSignal(nameof(Globals.ArrowFired), arrow, (int)team, position, direction);
    }
}
