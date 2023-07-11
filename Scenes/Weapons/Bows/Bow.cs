namespace DuchyOfThorns;

/// <summary>
/// Final bow class
/// </summary>
public partial class Bow : Projective
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
            deliverSound.Play();
            Vector2 direction = (WeaponDirection.GlobalPosition - EndOfWeapon.GlobalPosition).Normalized();
            globals.EmitSignal("ProjectileFired", (int)projectileType, damage, (int)team, EndOfWeapon.GlobalPosition, direction);
            SetCurrentAmmo(CurrentAmmo - 1);
            attackCooldown.Start();
            Idle();
    }
    public override void Walking() => animationPlayer.Play("Walk");
}
