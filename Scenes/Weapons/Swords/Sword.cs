namespace DuchyOfThorns;
public partial class Sword : Melee
{
    private AnimationPlayer animationPlayer;
    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    public override void Idle()
    {
        animationPlayer.Play("Idle");
    }
    public override void Attack()
    {
        delivered = false;
        animationPlayer.Play("Attack");
        attackSound.Play();
    }
    public override void Deliver()
    {
        animationPlayer.Play("Idle");
        attackCooldown.Start();
    }
    public override void Walking()
    {
        animationPlayer.Play("Walking");
    }
}
