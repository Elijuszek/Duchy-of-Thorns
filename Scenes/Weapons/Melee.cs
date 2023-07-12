namespace DuchyOfThorns;

/// <summary>
/// Intermediate class for all melee weapons
/// </summary>
public partial class Melee : Weapon
{
	[Export] protected CollisionShape2D collisionShape;

	public virtual void Idle()
	{
		IsAttacking = false;
	}
	public virtual void Attack() => IsAttacking = true;

	public virtual void Deliver() 
	{
		IsAttacking = false;
		attackCooldown.Start();
	}

	public virtual void Walking() => IsAttacking = false;

	public virtual void Area2DBodyEntered(Node body)
	{
		if (body is Actor actor && actor.GetTeam() != team)
		{
			actor.HandleHit(damage, GlobalPosition);
			deliverSound.Play();
		}
	}
}
