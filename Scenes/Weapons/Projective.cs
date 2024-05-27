namespace DuchyOfThorns;

/// <summary>
/// Intermediate class for bows
/// </summary>
public partial class Projective : Weapon
{
    [Signal] public delegate void WeaponAmmoChangedEventHandler(int newAmmo);
    [Signal] public delegate void PlayerReloadedEventHandler(int currentAmmo); // TODO move to weapons manager

    [Export] public bool CanAim { get; set; } = false;
    [Export] public int MaxAmmo { get; set; }
    [Export] public int CurrentAmmo { get; set; }
    [Export] public Marker2D WeaponDirection { get; set; }
    [Export] public Marker2D EndOfWeapon;
    [Export] protected ProjectileType projectileType;

    protected Globals globals;

    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>("/root/Globals");
    }

    public virtual void Attack()
    {
        attackSound.Play();
        IsAttacking = true;
    }

    public virtual void Deliver() 
    {
        attackSound.Stop();
        deliverSound.Play();
        Vector2 direction = (WeaponDirection.GlobalPosition - EndOfWeapon.GlobalPosition).Normalized();
        globals.EmitSignal("ProjectileFired", (int)projectileType, Damage, (int)team, EndOfWeapon.GlobalPosition, direction);
        SetCurrentAmmo(CurrentAmmo - 1);
        attackCooldown.Start();
        Idle();
        IsAttacking = false;
    }

    public virtual void Idle() => IsAttacking = false;
    public virtual void Walking() => IsAttacking = false;

    public override bool CanAttack()
    {
        return base.CanAttack() && CurrentAmmo > 0;
    }

    public void SetCurrentAmmo(int NewAmmo)
    {
        int actualAmmo = Mathf.Clamp(NewAmmo, 0, MaxAmmo);
        if (actualAmmo != CurrentAmmo)
        {
            CurrentAmmo = actualAmmo;
            EmitSignal(nameof(WeaponAmmoChanged), CurrentAmmo);
        }
    }
}
