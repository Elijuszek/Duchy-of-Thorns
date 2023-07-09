namespace DuchyOfThorns;

/// <summary>
/// Final melee weapon class
/// </summary>
public partial class Sword : Melee
{
    [Export] private AnimationPlayer animationPlayer;
    public override void Idle() => animationPlayer.Play("Idle");
    public override void Attack()
    {
        delivered = false;
        float speedScale = animationPlayer.GetAnimation("Attack").Length / AttackDuartion;
        animationPlayer.Play("Attack", -1, speedScale);
        attackSound.Play();
    }
    public override void Deliver()
    {
        animationPlayer.Play("Idle");
        attackCooldown.Start();
    }
    public override void Walking() => animationPlayer.Play("Walking");
}
