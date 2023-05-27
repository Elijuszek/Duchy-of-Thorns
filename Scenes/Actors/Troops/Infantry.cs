namespace DuchyOfThorns;

/// <summary>
/// Intermediate class for all infantry units
/// </summary>
public partial class Infantry : Troop
{
	public MeleeAI Ai { get; set; }
	protected Timer attackTimer;
	public override void _Ready()
	{
		base._Ready();
		Ai = GetNode<MeleeAI>("MeleeAI");
		attackTimer = GetNode<Timer>("AttackTimer");
	}
	public virtual void Attack() { GD.PrintErr("Calling Attack from Infantry class"); }
	protected virtual void AttackTimerTimeout() { GD.PrintErr("Calling AttackTimerTimeout from Infantry class"); }
}
