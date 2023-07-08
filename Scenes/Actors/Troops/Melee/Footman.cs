namespace DuchyOfThorns;

/// <summary>
/// Final melee unit class
/// </summary>
public partial class Footman : Infantry
{
	public override void _Ready()
	{
		base._Ready();
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		weapon = GetNode<Melee>("GoldenSword");
		weapon.Initialize(Team);
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}
	//public override void Attack()
	//{
	//	if (!isAttacking && weapon.CanAttack())
	//	{
	//		weapon.Attack();
	//	}
	//}
	//protected override void AttackTimerTimeout()
	//{
	//	weapon.Deliver();
	//	isAttacking = false;
	//}
}
