namespace DuchyOfThorns;

/// <summary>
/// Class for player functionality
/// </summary>
public partial class Player : Actor
{
    [Signal] public delegate void PlayerHealthChangedEventHandler(float newHealth);
    [Signal] public delegate void PLayerGoldChangedEventHandler(int newGold, int oldGold);
    [Signal] public delegate void DiedEventHandler();

    [Export] public WeaponManager WeaponsManager { get; set; }
    [Export] private AnimationPlayer animationPlayer;
    [Export] private RemoteTransform2D cameraTransform;
    [Export] private AudioStreamPlayer coinsSound;

    private Joystick movementJoystick;
    private Joystick attackJoystick;
    private Vector2 attackDirection = Vector2.Zero;
    private PackedScene damagePopup = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Popups/DamagePopup.tscn");
    private GUI gui;
    private Globals globals;

    public override void _Ready()
    {
        base._Ready();
        gui = GetParent().GetNode<GUI>("GUI");
        movementJoystick = gui.GetNode<Joystick>("MovementJoystick/Joystick_Button");
        attackJoystick = gui.GetNode<Joystick>("MarginContainer/Rows/MiddleRow/MarginContainer/AttackJoystick/Joystick_Button");
        globals = GetNode<Globals>("/root/Globals");
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // Movement calculation
        Direction = movementJoystick.GetValue();
        if (Direction == Vector2.Zero)
        {
            Direction = Input.GetVector("LEFT", "RIGHT", "UP", "DOWN");
        }

        Velocity = Direction * Stats.Speed;
        MoveAndSlide();

        // Attack
        if (attackJoystick.OngoingDrag != -1)
        {
            attackDirection = attackJoystick.GetValue();
            LookAt(attackDirection + GlobalPosition);
            if (WeaponsManager.Attack())
            {
                PlayAttackAnimation();
            }
        }
        if (WeaponsManager.CurrentWeapon.IsAttacking)
        {
            if (attackJoystick.OngoingDrag == -1)
            {
                WeaponsManager.Deliver();
                PlayIdle();
            }
            return;
        }

        // Walking
        if (Direction != Vector2.Zero)
        {
            LookAt(Direction + GlobalPosition);
            PlayWalking();
            WeaponsManager.Walking();
            return;
        }

        // Idle
        PlayIdle();
        WeaponsManager.Idle();
    }
    public override void HandleHit(float baseDamage, Vector2 impactPosition)
    {
        float damage = Mathf.Clamp(baseDamage - Stats.Armour, 0, 100);
        Stats.Health -= damage;
        //Stats.SetHealth(Stats.Health - damage); // UI won't go below 0hp
        EmitSignal(nameof(PlayerHealthChanged), Stats.Health);
        if (Stats.Health <= 0 && animationPlayer.CurrentAnimation != "Death")
        {
            SetPhysicsProcess(false);
            animationPlayer.Play("Death");
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
        //globals.Signals.EmitSignal("CoinsDroped", base.Stats.Gold / 3, GlobalPosition, true); // DefendMap
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

    public void Heal(float hp)
    {
        Stats.Health = Math.Clamp(Stats.Health + hp, 0, Stats.MaxHealth);
        EmitSignal(nameof(PlayerHealthChanged), Stats.Health);
    }


    public void CancelAttack() => WeaponsManager.CancelAttack();
    public void ChangeWeapon() => WeaponsManager.ChangeWeapon();
    public void PlayIdle() => animationPlayer.Play("Idle");
    public void PlayWalking() => animationPlayer.Play("Walking");

    private void PlayAttackAnimation()
    {
        switch (WeaponsManager.CurrentWeapon)
        {
            case Fists fists:
                animationPlayer.Play("Punsh");
                animationPlayer.SpeedScale = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.CurrentWeapon.AttackDuartion);
                break;
            case Bow:
                animationPlayer.Play("ShootingBow");
                animationPlayer.SpeedScale = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.CurrentWeapon.AttackDuartion);
                break;
            case Sword sword:
                animationPlayer.Play("SwordSwing");
                animationPlayer.SpeedScale = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.CurrentWeapon.AttackDuartion);
                break;
            case Spear spear:
                animationPlayer.Play("SpearAttack");
                animationPlayer.SpeedScale = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.CurrentWeapon.AttackDuartion);
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
        EmitSignal(nameof(PLayerGoldChanged), Stats.Gold, Stats.Gold);
        EmitSignal(nameof(PlayerHealthChanged), Stats.Health);
    }
    
}
