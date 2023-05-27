namespace DuchyOfThorns;

/// <summary>
/// Intermediate class for all melee weapons
/// </summary>
public partial class Melee : Weapon
{
	protected CollisionShape2D collisionShape;
	[Export] protected bool delivered = false;
	public override void _Ready()
	{
		base._Ready();
		collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
	}
	public override bool CanAttack()
	{
		return attackCooldown.IsStopped();
	}
	public virtual void Idle() { GD.PrintErr("Calling Idle from Melee class"); }
	public virtual void Attack() { GD.PrintErr("Calling Attack from Melee class"); }
	public virtual void Deliver() { GD.PrintErr("Calling Deliver from Melee class"); }
	public virtual void Walking() { GD.PrintErr("Calling Walking from Melee class"); }
	public virtual void Area2DBodyEntered(Node body)
	{
		if (body is Actor actor && actor.GetTeam() != team && !delivered)
		{
			actor.HandleHit(damage, GlobalPosition);
			deliverSound.Play();
			delivered = true;
		}
	}

}
