namespace DuchyOfThorns;

/// <summary>
/// Intermediate class for bows
/// </summary>
public partial class Bow : Weapon
{
    [Signal]
    public delegate void WeaponAmmoChangedEventHandler(int newAmmo);
    [Signal]
    public delegate void PlayerReloadedEventHandler(int currentAmmo);
    [Export] protected PackedScene Arrow;
    [Export] protected PackedScene FireArrow;
    [Export] protected bool OnFire = false;
    [Export] public bool CanAim { get; set; } = false;
    [Export] public int MaxAmmo { get; set; }
    [Export] public int CurrentAmmo { get; set; }
    public Marker2D BowDirection { get; set; }
    protected Marker2D endOfBow;
    protected Globals globals;
    public override void _PhysicsProcess(double delta) => base._Process(delta);
    public override void _Ready()
    {
        base._Ready();
        BowDirection = GetNode<Marker2D>("BowDirection");
        endOfBow = GetNode<Marker2D>("EndOfBow");
        globals = GetNode<Globals>("/root/Globals");
    }
    public virtual void Attack() { }
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