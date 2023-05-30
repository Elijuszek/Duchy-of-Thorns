namespace DuchyOfThorns;

/// <summary>
/// Class for player functionality
/// </summary>
public partial class Player : Actor
{
	[Signal]
	public delegate void PlayerHealthChangedEventHandler(float newHealth);
	[Signal]
	public delegate void PLayerGoldChangedEventHandler(int newGold, int oldGold);
	[Signal]
	public delegate void DiedEventHandler();

	[Export] float swingDuration = 0.5f; // TODO swing stab pierce hit
	[Export] float reloadDuration = 1f;

	private Joystick movementJoystick;
	private Joystick attackJoystick;
	private AnimationPlayer animationPlayer;
	private RemoteTransform2D cameraTransform;
	private AudioStreamPlayer coinsSound;
	private Vector2 movementDirection = Vector2.Zero;
	private Vector2 attackDirection = Vector2.Zero;
	private PackedScene damagePopup = (PackedScene)ResourceLoader.Load("res://Scenes/UI/Popups/DamagePopup.tscn");
	private GUI gui;
	private Globals globals;
	public WeaponManager WeaponsManager { get; set; }
	public override void _Ready()
	{
		base._Ready();
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		coinsSound = GetNode<AudioStreamPlayer>("CoinsSound");
		WeaponsManager = GetNode<WeaponManager>("WeaponsManager");
		WeaponsManager.Initialize(GetTeam());
		gui = GetParent().GetNode<GUI>("GUI");
		movementJoystick = gui.GetNode<Joystick>("MovementJoystick/Joystick_Button");
		attackJoystick = gui.GetNode<Joystick>("MarginContainer/Rows/MiddleRow/MarginContainer/AttackJoystick/Joystick_Button");
		cameraTransform = GetNode<RemoteTransform2D>("CameraTransform");
		Stats = GetNode<Stats>("Stats");
		globals = GetNode<Globals>("/root/Globals");
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (!WeaponsManager.IsAttacking)
		{
			if (movementDirection != Vector2.Zero)
			{
				PlayWalking();
			}
			else
			{
				PlayIdle();
			}
		}
		attackDirection = attackJoystick.GetValue();
		movementDirection = movementJoystick.GetValue();

		Velocity = movementDirection * Stats.Speed;
		if (attackJoystick.OngoingDrag != -1)
		{
			LookAt(GlobalPosition + attackDirection);
			if (!WeaponsManager.IsAttacking && WeaponsManager.currentWeapon.CanAttack())
			{
				WeaponsManager.Attack();
				PlayAttackAnimation();
			}
		}
		else if (attackJoystick.OngoingDrag == -1 && WeaponsManager.IsAttacking)
		{
			WeaponsManager.Deliver();
		}
		else
		{
			LookAt(GlobalPosition + movementDirection);
		}
	}
	public override void HandleHit(float baseDamage, Vector2 impactPosition)
	{
		float damage = Mathf.Clamp(baseDamage - Stats.Armour, 0, 100);
		Stats.Health -= damage;
		//Stats.SetHealth(Stats.Health - damage); // UI won't go below 0hp
		EmitSignal(nameof(PlayerHealthChanged), Stats.Health);
		if (Stats.Health <= 0)
		{
			Die();
		}
		else
		{
			Blood blood = bloodScene.Instantiate() as Blood;
			GetParent().AddChild(blood);
			blood.GlobalPosition = GlobalPosition;
			blood.Rotation = impactPosition.DirectionTo(GlobalPosition).Angle();


			DamagePopup popup = damagePopup.Instantiate() as DamagePopup;
			popup.Amount = (int)damage;
			popup.Type = "Damage";
			AddChild(popup);
		}
	}
	public override void Die()
	{
		//globals.EmitSignal("CoinsDroped", base.Stats.Gold / 3, GlobalPosition); // DefendMap
		EmitSignal(nameof(Died));
		QueueFree();

	}
	public void GetGold(int gold)
	{
		int oldGold = Stats.Gold;
		Stats.Gold += gold;
		coinsSound.Play();
		EmitSignal(nameof(PLayerGoldChanged), Stats.Gold, oldGold);
	}
	public void SetGold(int gold)
	{
		Stats.Gold = gold;
		EmitSignal(nameof(PLayerGoldChanged), gold, gold);
	}
	public void CancelAttack() => WeaponsManager.CancelAttack();
    public void ChangeWeapon() => WeaponsManager.ChangeWeapon();
	public void PlayIdle()
	{
		animationPlayer.Play("Idle");
		WeaponsManager.Idle();
	}
	public void PlayWalking()
	{
		animationPlayer.Play("Walking");
		WeaponsManager.Walking();
	}
	private void PlayAttackAnimation()
	{
		switch (WeaponsManager.currentWeapon)
		{
			case Fists fists:
				animationPlayer.Play("Punsh");
				animationPlayer.SpeedScale = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.currentWeapon.AttackDuartion);
				break;
			case Bow bow:
				animationPlayer.Play("ShootingBow");
				animationPlayer.SpeedScale = 1;
				break;
			case Sword sword:
				animationPlayer.Play("SwordSwing");
				animationPlayer.SpeedScale = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.currentWeapon.AttackDuartion);
				break;
			case Spear spear:
				animationPlayer.Play("SpearAttack");
				animationPlayer.SpeedScale = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.currentWeapon.AttackDuartion);
				break;
		}
	}
	public void SetCameraTransform(NodePath cameraPath) => cameraTransform.RemotePath = cameraPath;
    public Godot.Collections.Dictionary<string, Variant> Save()
	{
		return new Godot.Collections.Dictionary<string, Variant>()
		{
			{ "Filename", SceneFilePath }, // was FileName
			{ "Parent", GetParent().GetPath() },
			{ "PosX", Position.X },
			{ "PosY", Position.Y },
			{ "Stats.Health", Stats.Health },
			{ "Stats.MaxHealth", Stats.MaxHealth },
			{ "Stats.DamageMultiplier", Stats.DamageMultiplier },
			{ "Stats.Armour", Stats.Armour },
			{ "Stats.Speed", Stats.Speed },
			{ "Stats.Gold", Stats.Gold },
			{ "WeaponsManager", WeaponsManager.Save() }
		};
	}
	public virtual void Load(Godot.Collections.Dictionary<string, Variant> data)
	{
		Position = new Vector2((float)data["PosX"], (float)data["PosY"]);
		Stats.Health = (float)data["Stats.Health"];
		Stats.MaxHealth = (float)data["Stats.MaxHealth"];
		Stats.DamageMultiplier = (float)data["Stats.DamageMultiplier"];
		Stats.Armour = (float)data["Stats.Armour"];
		Stats.Speed = (float)data["Stats.Speed"];
		Stats.Gold = (int)data["Stats.Gold"];
		WeaponsManager.Load(new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)data["WeaponsManager"]));
		WeaponsManager.Initialize((int)team.team);
		EmitSignal(nameof(PLayerGoldChanged), Stats.Gold, Stats.Gold);
		EmitSignal(nameof(PlayerHealthChanged), Stats.Health);
	}
}