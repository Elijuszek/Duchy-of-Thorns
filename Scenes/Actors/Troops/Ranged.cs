namespace DuchyOfThorns;

public partial class Ranged : Troop
{
    public RangedAI Ai { get; set; }
    protected Timer attackTimer;
    public override void _Ready()
    {
        base._Ready();
        Ai = GetNode<RangedAI>("RangedAI");
        attackTimer = GetNode<Timer>("AttackTimer");
    }
    public virtual void Attack() { GD.PrintErr("Calling Attack from Ranged class"); }
    protected virtual void AttackTimerTimeout() { GD.PrintErr("Calling AttackTimerTimeout from Ranged class"); }
}
