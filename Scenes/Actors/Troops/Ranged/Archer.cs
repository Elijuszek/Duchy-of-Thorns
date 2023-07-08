namespace DuchyOfThorns;

/// <summary>
/// Final ranged unit class
/// </summary>
public partial class Archer : Ranged
{
    private AnimationPlayer animationPlayer;
    private Bow weapon;
    private bool isAttacking = false;
    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        weapon = GetNode<Bow>("SimpleBow");
        weapon.Initialize(Team);
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (isAttacking)
        {
            animationPlayer.Play("ShootingBow");
        }
        else if (Velocity != Vector2.Zero)
        {
            animationPlayer.Play("Walking");
        }
        else
        {
            animationPlayer.Play("Idle");
        }
    }
    //public override void Attack()
    //{
    //    if (!isAttacking && weapon.CanAttack())
    //    {
    //        isAttacking = true;
    //        weapon.Attack();
    //        attackTimer.Start();
    //    }
    //}
    //protected override void AttackTimerTimeout()
    //{
    //    weapon.Deliver();
    //    isAttacking = false;
    //}
}
