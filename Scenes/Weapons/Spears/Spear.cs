namespace DuchyOfThorns;

/// <summary>
/// Final melee weapon class
/// </summary>
public partial class Spear : Melee
{
	[Export] private float knockBack = 5;
	[Export] private AnimationPlayer animationPlayer;
	public override void _Ready()
	{
		base._Ready();
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	public override void Idle() => animationPlayer.Play("Idle");
	public override void Attack()
	{
		animationPlayer.Play("Attack");
	}
	public override void Deliver()
	{
		animationPlayer.Play("Idle");
		attackCooldown.Start();
	}
	public override void Walking() => animationPlayer.Play("Walking");
	public override void Area2DBodyEntered(Node body)
	{
		if (body is Actor actor && actor.GetTeam() != team)
		{
			actor.HandleHit(damage, GlobalPosition);
			deliverSound.Play();
			actor.HandleKnockback(knockBack, GlobalPosition);
		}
	}
}
