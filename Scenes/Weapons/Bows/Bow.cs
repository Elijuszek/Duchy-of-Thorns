namespace DuchyOfThorns;

/// <summary>
/// Final bow class
/// </summary>
public partial class Bow : Projective
{
    [Export] private AnimationPlayer animationPlayer;

    public override void Idle()
    {
        base.Idle();
        animationPlayer.Play("Idle");
    }

    public override bool CanAttack() => base.CanAttack();

    public override void Attack()
    {
        base.Attack();
        animationPlayer.Play("Attack");
    }

    public override void Deliver() => base.Deliver();

    public override void Walking()
    {
        base.Walking();
        animationPlayer.Play("Walk");
    }
}
