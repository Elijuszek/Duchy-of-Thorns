namespace DuchyOfThorns;

/// <summary>
/// Intermediate class for all troops
/// </summary>
public partial class Troop : Actor
{
	[Signal] public delegate void DiedEventHandler();

    [Export] protected AnimationPlayer animationPlayer;

    public Vector2 Destination { get; set; }
    private PackedScene damagePopup;
	private Globals globals;
	public override void _Ready()
	{
		base._Ready();
		damagePopup = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Popups/DamagePopup.tscn");
		globals = GetNode<Globals>("/root/Globals");
	}
	public override void HandleHit(float baseDamage, Vector2 impactPosition)
	{
		float damage = Mathf.Clamp(baseDamage - Stats.Armour, 0, Stats.MaxHealth);
		Stats.Health -= damage;
		if (Stats.Health <= 0)
		{
			animationPlayer.Play("Death");
		}
		else
		{
			if (damage > 9)
			{
				Blood blood = bloodScene.Instantiate() as Blood;
				GetParent().AddChild(blood);
				blood.GlobalPosition = GlobalPosition;
				blood.Rotation = impactPosition.DirectionTo(GlobalPosition).Angle();
			}
			DamagePopup popup = damagePopup.Instantiate() as DamagePopup;
			popup.Amount = (int)damage;
			popup.Type = "Damage";
			AddChild(popup);
		}
	}
	public override void Die()
	{
		if (Stats.Gold > 0)
		{
			Random rand = new Random();
			globals.EmitSignal("CoinsDroped", rand.Next(1, Stats.Gold), GlobalPosition, true);
		}
		EmitSignal(nameof(Died));
		QueueFree();
	}
}
