namespace DuchyOfThorns;

/// <summary>
/// Class for fireplace functionality
/// </summary>
public partial class Fireplace : StaticBody2D
{
    [Export] AudioStreamPlayer2D firePlayer;
    [Export] AudioStreamPlayer2D setOnFirePlayer;

    protected Globals globals;

    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>("/root/Globals");
    }

    public void SetOnFire(float damage, Team team, Vector2 position, Vector2 direction)
    {
        setOnFirePlayer.Play();
        globals.EmitSignal("ProjectileFired", (int)ProjectileType.FIRE_ARROW, damage, (int)team, position, direction);
    }
}
