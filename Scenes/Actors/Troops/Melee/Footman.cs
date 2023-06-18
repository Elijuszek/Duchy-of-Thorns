namespace DuchyOfThorns;

/// <summary>
/// Final melee unit class
/// </summary>
public partial class Footman : Infantry
{
	private AnimationPlayer animationPlayer;
	private Melee weapon;
	private bool isAttacking = false;
	public override void _Ready()
	{
		base._Ready();
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		weapon = GetNode<Melee>("GoldenSword");
		Ai.Initialize(weapon, team.team, GetNode<TileMap>("/root/World/TileMap"));
		weapon.Initialize(team.team);
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (isAttacking)
		{
			animationPlayer.Play("Attack");
		}
		else if (Velocity != Vector2.Zero)
		{
			animationPlayer.Play("Walking");
			weapon.Walking();
		}
		else
		{
			animationPlayer.Play("Idle");
			weapon.Idle();
		}
	}
	public override void Attack()
	{
		if (!isAttacking && weapon.CanAttack())
		{
			isAttacking = true;
			weapon.Attack();
			attackTimer.Start();
		}
	}
	protected override void AttackTimerTimeout()
	{
		weapon.Deliver();
		isAttacking = false;
	}
}
