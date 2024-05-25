namespace DuchyOfThorns;

/// <summary>
/// Class for managing player weapons
/// </summary>
public partial class WeaponManager : Node2D
{
    [Signal] public delegate void WeaponChangedEventHandler(Weapon newWeapon);

    [Export] public Team Team { get; set; }
    [Export] public Weapon CurrentWeapon { get; set; }
    [Export] private Aiming aiming;
    public Weapon[] weapons;

    public override void _Ready() // TODO
    {
        weapons = new Weapon[3];
        int i = 0;
        foreach (Weapon weapon in GetChildren().OfType<Weapon>())
        {
            weapons[i++] = weapon;
            weapon.Initialize(Team);
            weapon.Hide();
        }
        CurrentWeapon.Show();
    }

    public void AddWeapon(Weapon weapon, int index)
    {
        if (weapons[index] != null)
        {
            weapons[index].QueueFree();
        }
        AddChild(weapon);
        weapon.Initialize(Team);
        weapons[index] = weapon;
        weapon.Hide();
    }

    public Weapon GetCurrentWeapon() => CurrentWeapon;

    public void ChangeWeapon() // Three main weapons Fists, melee, and bow / staff
    {
        int index = Array.IndexOf(weapons, CurrentWeapon);
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
        if (CurrentWeapon == weapon) return;
        CancelAttack();
        CurrentWeapon.Hide();
        weapon.Show();
        CurrentWeapon = weapon;
        EmitSignal(nameof(WeaponChanged), CurrentWeapon);
    }

    public void Idle()
    {
        if (CurrentWeapon is Melee melee)
        {
            melee.Idle();
        }
        else if (CurrentWeapon is Projective projective)
        {
            projective.Idle();
        }
    }

    public bool Attack()
    {

        if (!CurrentWeapon.CanAttack())
        {
            return false;
        }

        switch (CurrentWeapon)
        {
            case Melee melee:
                melee.Attack();
                break;

            case Projective projective:
                if (projective.CanAim)
                {
                    aiming.StartAiming(2f, 0.1f, 2f, projective.AttackDuartion); // start, end, duration, delay
                }
                projective.Attack();
                break;
            default:
                GD.PrintErr("Invalid weapon type in weapon manager Attack method: ", CurrentWeapon.GetType());
                return false;
        }
        return true;
    }

    public void CancelAttack()
    {
        Idle();
        aiming.Cancel();
        CurrentWeapon.StartCooldown();
    }

    public void Deliver()
    {
        if (CurrentWeapon is Melee melee)
        {
            melee.Deliver();
        }
        else if (CurrentWeapon is Projective projective)
        {
            projective.WeaponDirection.Position = new Vector2(projective.WeaponDirection.Position.X, aiming.GetDirection());
            projective.Deliver();
        }
    }

    public void Walking()
    {
        if (CurrentWeapon is Melee melee)
        {
            melee.Walking();
        }
        else if (CurrentWeapon is Projective projective)
        {
            projective.Walking();
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
                data.Add("PrimaryFilename", weapons[1].SceneFilePath);
            }
            else if (weapons[1] is Projective projective)
            {
                data.Add("PrimaryType", "Bow");
                data.Add("PrimaryFilename", weapons[1].SceneFilePath);
                data.Add("PrimaryAmmo", projective.CurrentAmmo);
                data.Add("PrimaryMaxAmmo", projective.MaxAmmo);
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
                data.Add("SecondaryFilename", weapons[2].SceneFilePath);
            }
            else if (weapons[2] is Projective projective)
            {
                data.Add("SecondaryType", "Bow");
                data.Add("SecondaryFilename", weapons[2].SceneFilePath);
                data.Add("SecondaryAmmo", projective.CurrentAmmo);
                data.Add("SecondaryMaxAmmo", projective.MaxAmmo);
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
            var newObjectScene = ResourceLoader.Load<PackedScene>(data["PrimaryFilename"].ToString());
            AddWeapon(newObjectScene.Instantiate<Weapon>(), 1);
        }
        else if ((string)data["PrimaryType"] == "Bow")
        {
            var newObjectScene = ResourceLoader.Load<PackedScene>(data["PrimaryFilename"].ToString());
            Projective projective = newObjectScene.Instantiate<Projective>();
            projective.CurrentAmmo = (int)(data["PrimaryAmmo"]);
            projective.MaxAmmo = (int)data["PrimaryMaxAmmo"];
            AddWeapon(projective, 1);
        }
        if ((string)data["SecondaryType"] == "Melee")
        {
            var newObjectScene = ResourceLoader.Load<PackedScene>(data["SecondaryFilename"].ToString());
            AddWeapon(newObjectScene.Instantiate() as Weapon, 2);
        }
        else if ((string)data["SecondaryType"] == "Bow")
        {
            var newObjectScene = ResourceLoader.Load<PackedScene>(data["SecondaryFilename"].ToString());
            Projective projective = newObjectScene.Instantiate<Projective>();
            projective.CurrentAmmo = (int)(data["SecondaryAmmo"]);
            projective.MaxAmmo = (int)(data["SecondaryMaxAmmo"]);
            AddWeapon(projective, 2);
        }
    }
}
