using DuchyofThorns.Scenes.Globals;

namespace DuchyOfThorns;

/// <summary>
/// Base class for all weapons
/// </summary>
public partial class Weapon : Node2D
{
    [Export] public float AttackDuartion { get; set; } = 0.7f;
    [Export] protected float damage;
    protected Timer attackCooldown;
    protected AudioStreamPlayer2D deliverSound;
    protected AudioStreamPlayer2D attackSound;
    protected Team team;
    public virtual bool CanAttack()
    {
        GD.PrintErr("Weapon CanAttack is not possible!");
        return false;
    }
    public override void _Ready()
    {
        attackCooldown = GetNode<Timer>("AttackCooldown");
        deliverSound = GetNode<AudioStreamPlayer2D>("DeliverSound");
        attackSound = GetNode<AudioStreamPlayer2D>("AttackSound");
    }
    public void Initialize(Team team) => this.team = team;
    public void StartCooldown() => attackCooldown.Start();
}
