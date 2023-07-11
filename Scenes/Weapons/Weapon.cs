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
    public bool IsAttacking { get; set; } = false;
    protected Team team;
    public virtual bool CanAttack()
    {
        return !IsAttacking && attackCooldown.IsStopped();
    }
    public void Initialize(Team team) => this.team = team;
    public void StartCooldown() => attackCooldown.Start();
}
