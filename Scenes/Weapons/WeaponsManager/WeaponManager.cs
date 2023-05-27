namespace DuchyOfThorns;

/// <summary>
/// Class for managing player weapons
/// </summary>
public partial class WeaponManager : Node2D
{
	[Signal]
	public delegate void WeaponChangedEventHandler(Weapon newWeapon);
	public Weapon currentWeapon { get; set; }
	public Weapon[] weapons;
	private Aiming aiming;
	private Timer attackTimer;
	public bool IsAttacking { get; set; } = false;
	public override void _Ready() // TODO
	{
		currentWeapon = GetNode<Weapon>("Fists");
		attackTimer = GetNode<Timer>("AttackTimer");
		weapons = new Weapon[3];
		int i = 0;
		foreach (Weapon weapon in GetChildren().OfType<Weapon>())
		{
			weapons[i++] = weapon;
			weapon.Hide();
		}
		currentWeapon.Show();
		aiming = GetNode<Aiming>("Aiming");
	}
	public void Initialize(int team)
	{
		foreach (Weapon weapon in weapons)
		{
			if (weapon != null)
			{
				weapon.Initialize(team);
			}
		}
	}
	public void AddWeapon(Weapon weapon, int index)
	{
		if (weapons[index] != null)
		{
			weapons[index].QueueFree();
		}
		AddChild(weapon);
		weapons[index] = weapon;
		weapon.Hide();
	}
	public Weapon GetCurrentWeapon() => currentWeapon;
    public void ChangeWeapon() // Three main weapons Fists, melee, and bow / staff
	{
		int index = Array.IndexOf(weapons, currentWeapon);
		for (int i = index; i < weapons.Length - 1; i++)
		{
			if (weapons[i + 1] != null)
			{
				SwitchWeapon(weapons[i + 1]);
				return;
			}
		}
		SwitchWeapon(weapons[0]);
	}
	private void SwitchWeapon(Weapon weapon)
	{
		if (currentWeapon == weapon) return;
		CancelAttack();
		currentWeapon.Hide();
		weapon.Show();
		currentWeapon = weapon;
		EmitSignal(nameof(WeaponChanged), currentWeapon);
	}
	public void Idle()
	{
		if (currentWeapon is Melee melee)
		{
			melee.Idle();
		}
		else if (currentWeapon is Bow bow)
		{
			bow.Idle();
		}
	}
	public void Attack()
	{
		if (!IsAttacking && currentWeapon.CanAttack())
		{
			if (currentWeapon is Melee melee)
			{
				IsAttacking = true;
				attackTimer.Start(melee.AttackDuartion);
				melee.Attack();
			}
			else if (currentWeapon is Bow bow)
			{
				IsAttacking = true;
				attackTimer.Start(bow.AttackDuartion);
				if (bow.CanAim)
				{
					aiming.StartAiming(2f, 0.1f, 2f, bow.AttackDuartion); // start, end, duration, delay
				}
				bow.Attack();
			}
		}
	}
	public void CancelAttack()
	{
		IsAttacking = false;
		Player player = GetParent() as Player;
		player.PlayIdle();
		aiming.Cancel();
		currentWeapon.StartCooldown();
	}
	public void Deliver()
	{
		if (IsAttacking == true)
		{
			if (currentWeapon is Melee melee && attackTimer.IsStopped())
			{
				IsAttacking = false;
				melee.Deliver();
			}
			else if (currentWeapon is Bow bow && bow.CanAttack() && attackTimer.IsStopped())
			{
				bow.BowDirection.Position = new Vector2(bow.BowDirection.Position.X, aiming.GetDirection());
				bow.Deliver();
				IsAttacking = false;
				Idle();
			}
		}
	}
	public void Walking()
	{
		if (currentWeapon is Melee melee)
		{
			melee.Walking();
		}
		else if (currentWeapon is Bow bow)
		{
			bow.Walking();
		}
	}
	private void AttackTimerTimeout()
	{
		if (currentWeapon is Melee)
		{
			Deliver();
		}
	}
	public Godot.Collections.Dictionary<string, Variant> Save()
	{
		Godot.Collections.Dictionary<string, Variant> data = new Godot.Collections.Dictionary<string, Variant>();
		if (weapons[1] != null)
		{
			if (weapons[1] is Melee)
			{
				data.Add("PrimaryType", "Melee");
				data.Add("PrimaryFilename", weapons[1].Name);
			}
			else if (weapons[1] is Bow bow)
			{
				data.Add("PrimaryType", "Bow");
				data.Add("PrimaryFilename", weapons[1].Name);
				data.Add("PrimaryAmmo", bow.CurrentAmmo);
				data.Add("PrimaryMaxAmmo", bow.MaxAmmo);
			}
		}
		else
		{
			data.Add("PrimaryType", "None");
		}
		if (weapons[2] != null)
		{
			if (weapons[2] is Melee)
			{
				data.Add("SecondaryType", "Melee");
				data.Add("SecondaryFilename", weapons[2].Name);
			}
			else if (weapons[2] is Bow bow)
			{
				data.Add("SecondaryType", "Bow");
				data.Add("SecondaryFilename", weapons[2].Name);
				data.Add("SecondaryAmmo", bow.CurrentAmmo);
				data.Add("SecondaryMaxAmmo", bow.MaxAmmo);
			}
		}
		else
		{
			data.Add("SecondaryType", "None");
		}
		return data;
	}
	public void Load(Godot.Collections.Dictionary<string, Variant> data)
	{
		if ((string)data["PrimaryType"] == "Melee")
		{
			var newObjectScene = (PackedScene)ResourceLoader.Load(data["PrimaryFilename"].ToString());
			AddWeapon(newObjectScene.Instantiate() as Weapon, 1);
		}
		else if ((string)data["PrimaryType"] == "Bow")
		{
			var newObjectScene = (PackedScene)ResourceLoader.Load(data["PrimaryFilename"].ToString());
			Bow bow = newObjectScene.Instantiate() as Bow;
			bow.CurrentAmmo = Convert.ToInt32(data["PrimaryAmmo"]);
			bow.MaxAmmo = Convert.ToInt32(data["PrimaryMaxAmmo"]);
			AddWeapon(bow, 1);
		}
		if ((string)data["SecondaryType"] == "Melee")
		{
			var newObjectScene = (PackedScene)ResourceLoader.Load(data["SecondaryFilename"].ToString());
			AddWeapon(newObjectScene.Instantiate() as Weapon, 2);
		}
		else if ((string)data["SecondaryType"] == "Bow")
		{
			var newObjectScene = (PackedScene)ResourceLoader.Load(data["SecondaryFilename"].ToString());
			Bow bow = newObjectScene.Instantiate() as Bow;
			bow.CurrentAmmo = Convert.ToInt32(data["SecondaryAmmo"]);
			bow.MaxAmmo = Convert.ToInt32(data["SecondaryMaxAmmo"]);
			AddWeapon(bow, 2);
		}
		Initialize((int)Team.TeamName.PLAYER);
	}
}
