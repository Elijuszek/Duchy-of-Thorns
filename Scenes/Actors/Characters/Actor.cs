namespace DuchyOfThorns;

/// <summary>
/// Base class for all characters in the game
/// </summary>
public partial class Actor : CharacterBody2D
{
	//public Vector2 Velocity { get; set; } = Vector2.Zero; GODOT4
	public Stats Stats { get; set; }
	protected Team team;
	public Vector2 Direction { get; set; }

	protected CollisionShape2D collisionShape;
	protected PackedScene bloodScene;
	private Vector2 knockback = Vector2.Zero;
	public override void _Ready()
	{
		base._Ready();
		Stats = GetNode<Stats>("Stats");
		team = GetNode<Team>("Team");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		bloodScene = ResourceLoader.Load<PackedScene>("res://Material/Particles/Impact/Blood.tscn");
	}
	public virtual void HandleHit(float baseDamage, Vector2 impactPosition) => GD.PrintErr("Calling HandleHit from Actor class");
    public virtual void Die() => GD.PrintErr("Calling Die from Actor class");
    public void HandleKnockback(float amount, Vector2 impactPosition)
	{
		Vector2 direction = (impactPosition.DirectionTo(GlobalPosition));
		float strenght = Mathf.Clamp(amount, 5f, 20000f);
		knockback = direction * strenght;
	}
	public Team.TeamName GetTeam() => team.team;
    public bool HasReachedPosition(Vector2 location) => GlobalPosition.DistanceTo(location) < 50;
    public Vector2 VelocityToward(Vector2 location) => GlobalPosition.DirectionTo(location) * Stats.Speed;
    public void RotateToward(Vector2 location)
	{
		float r = Mathf.LerpAngle(Rotation, GlobalPosition.DirectionTo(location).Angle(), 0.1f);
		if (r < -Math.PI)
		{
			r += (float)Math.PI * 2;
		}
		else if (r > Math.PI)
		{
			r -= (float)Math.PI * 2;
		}
		Rotation = r;
	}
	public void RotateTowardLerp(Vector2 location) => Rotation = Mathf.LerpAngle(Rotation, GlobalPosition.DirectionTo(location).Angle(), 0.1f);
}
