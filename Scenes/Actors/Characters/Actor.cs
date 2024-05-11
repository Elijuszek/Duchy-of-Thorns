namespace DuchyOfThorns;

/// <summary>
/// Base class for all characters in the game
/// </summary>
public partial class Actor : CharacterBody2D
{
	[Export] public Team Team { get; set; } = Team.NEUTRAL;
	[Export] public Stats Stats { get; set; }
    [Export] protected CollisionShape2D collisionShape;
    public Vector2 Direction { get; set; }
	protected PackedScene bloodScene;
	protected Vector2 knockback = Vector2.Zero;
	public override void _Ready()
	{
		base._Ready();
		bloodScene = ResourceLoader.Load<PackedScene>("res://Material/Particles/Impact/Blood.tscn");
	}
	public virtual void HandleHit(float baseDamage, Vector2 impactPosition) => GD.PrintErr("Calling HandleHit from Actor class");
    public virtual void Die() => GD.PrintErr("Calling Die from Actor class");
    public void HandleKnockback(float amount, Vector2 impactPosition)
	{
		Vector2 direction = (impactPosition.DirectionTo(GlobalPosition));
		float strenght = Mathf.Clamp(amount, 5f, 20000f);
		knockback = direction * strenght;
		Tween tween = CreateTween();
        tween.TweenProperty(this, "knockback", Vector2.Zero, 1f);
        Velocity += knockback;
    }
	public Team GetTeam() => Team;
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
	public void RotateToward(float angle)
	{
        float r = Mathf.LerpAngle(Rotation, angle, 0.1f);
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
}
