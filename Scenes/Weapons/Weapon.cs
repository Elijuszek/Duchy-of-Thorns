namespace DuchyOfThorns;

/// <summary>
/// Base class for all weapons
/// </summary>
public partial class Weapon : Node2D
{
    [Export] public float AttackDuartion { get; set; } = 0.7f;
    [Export] protected float damage;
    [Export] protected Timer attackCooldown;
    [Export] protected AudioStreamPlayer2D deliverSound;
    [Export] protected AudioStreamPlayer2D attackSound;
    protected Team team;
    public virtual bool CanAttack()
    {
        GD.PrintErr("Weapon CanAttack is not possible!");
        return false;
    }
    public void Initialize(Team team) => this.team = team;
    public void StartCooldown() => attackCooldown.Start();
}
