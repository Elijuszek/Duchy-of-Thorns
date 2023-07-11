namespace DuchyOfThorns;

/// <summary>
/// Final melee weapon class
/// </summary>
public partial class Sword : Melee
{
    [Export] private AnimationPlayer animationPlayer;
    public override bool CanAttack()
    {
        return base.CanAttack();
    }
    public override void Idle()
    {
        base.Idle();
        animationPlayer.Play("Idle");
    }
    public override void Attack()
    {
        base.Attack();
        float speedScale = animationPlayer.GetAnimation("Attack").Length / AttackDuartion;
        animationPlayer.Play("Attack", -1, speedScale);
        attackSound.Play();
    }
    public override void Deliver()
    {
        base.Deliver();
        attackCooldown.Start();
    }
    public override void Walking()
    {
        base.Walking();
        animationPlayer.Play("Walking");
    }
}
