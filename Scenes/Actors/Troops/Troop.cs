using Godot;

namespace DuchyOfThorns;

/// <summary>
/// Intermediate class for all troops
/// </summary>
public partial class Troop : Actor, IPoolable
{
    public event RemovedFromSceneEventHandler RemovedFromScene;

    [Export] protected AnimationPlayer animationPlayer;

    public TroopState CurrentState { get; set; } = TroopState.ADVANCE;
    public Vector2 Origin { get; set; }
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
			// TODO: animation doesn't play (FootmanEnemy)
			// animationPlayer.Play("Death");
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
			globals.EmitSignal("CoinsDroped", Stats.Gold, GlobalPosition, true);
		}
        RemoveFromScene();
	}
    public virtual void SetState(TroopState newState)
	{
        if (newState == CurrentState)
        {
            return;
        }
    }

    public virtual void AddToScene()
    {
        collisionShape.SetDeferred("disabled", false);
        SetPhysicsProcess(true);
		Show();
    }

    public virtual void RemoveFromScene()
    {
		// TODO: navigation2D is still active (Debug from FootmanEnemy)
		GlobalPosition = Vector2.Zero;
		CurrentState = TroopState.NONE;
        collisionShape.SetDeferred("disabled", true);
		SetPhysicsProcess(false);
		Hide();
        if (RemovedFromScene != null)
        {
            RemovedFromScene(this);
        }
    }
}
