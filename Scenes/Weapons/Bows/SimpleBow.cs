namespace DuchyOfThorns;

/// <summary>
/// Final bow class
/// </summary>
public partial class SimpleBow : Bow
{
    [Export] private AnimationPlayer animationPlayer;
    public override void Idle() => animationPlayer.Play("Idle");
    public override void Attack()
    {
        animationPlayer.Play("Attack");
        if (CanAim)
        {
            attackSound.Play();
        }
    }
    public override void Deliver()
    {
        if (!OnFire && Arrow != null)
        {
            deliverSound.Play();
            Node arrow = Arrow.Instantiate();
            Vector2 direction = (BowDirection.GlobalPosition - endOfBow.GlobalPosition).Normalized();
            globals.EmitSignal(nameof(Globals.ArrowFired), arrow, (int)team, endOfBow.GlobalPosition, direction);
            SetCurrentAmmo(CurrentAmmo - 1);
            attackCooldown.Start();
            Idle();
        }
        else if (OnFire && FireArrow != null)
        {
            deliverSound.Play();
            Node arrow = FireArrow.Instantiate();
            Vector2 direction = (BowDirection.GlobalPosition - endOfBow.GlobalPosition).Normalized();
            globals.EmitSignal(nameof(Globals.ArrowFired), arrow, (int)team, endOfBow.GlobalPosition, direction);
            SetCurrentAmmo(CurrentAmmo - 1);
            attackCooldown.Start();
            Idle();
        }
    }
    public override void Walking() => animationPlayer.Play("Walking");
}
