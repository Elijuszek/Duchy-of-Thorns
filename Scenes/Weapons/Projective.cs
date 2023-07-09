using Godot.Collections;

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
    [Export] protected Marker2D EndOfWeapon;
    [Export] protected ProjectileType projectileType;

    protected Globals globals;
    public override void _PhysicsProcess(double delta) => base._Process(delta);
    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>("/root/Globals");
    }
    public virtual void Attack() 
    { 
    }
    public virtual void Deliver() { }
    public virtual void Idle() { }
    public virtual void Walking() { }
    public override bool CanAttack()
    {
        return attackCooldown.IsStopped() && CurrentAmmo > 0;
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
