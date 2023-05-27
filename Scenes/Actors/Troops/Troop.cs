namespace DuchyOfThorns;

public partial class Troop : Actor
{
	[Signal] public delegate void DiedEventHandler();
	PackedScene damagePopup;
	Globals globals;
	public override void _Ready()
	{
		base._Ready();
		damagePopup = (PackedScene)ResourceLoader.Load("res://Scenes/UI/Popups/DamagePopup.tscn");
		globals = GetNode<Globals>("/root/Globals");
	}
	public override void HandleHit(float baseDamage, Vector2 impactPosition)
	{
		float damage = Mathf.Clamp(baseDamage - Stats.Armour, 0, Stats.MaxHealth);
		Stats.Health -= damage;
		if (Stats.Health <= 0)
		{
			Die();
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
			globals.EmitSignal("CoinsDroped", rand.Next(1, Stats.Gold), GlobalPosition);
		}
		EmitSignal(nameof(Died));
		QueueFree();
	}
}
