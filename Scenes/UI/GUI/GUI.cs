namespace DuchyOfThorns;

/// <summary>
/// Class for waiting layer stats changes and updating GUI accordingly
/// Showing stats and ammo, receiving movement and attack input
/// </summary>
public partial class GUI : CanvasLayer
{
    [Signal] public delegate void NewWaveStartedEventHandler();

    [Export] private AnimationPlayer animationPlayer;
    [Export] private ProgressBar healthBar;
    [Export] private Label currentAmmo;
    [Export] private Label currentHealth;
    [Export] private Label maxHealth;
    [Export] private Button newWaveButton;
    [Export] private Label currentGold;

    private Tween healthTween;
    private Tween goldTween;
    private Player player;
    private Color originalColor = new Color("#7c1616");
    private Color hightlightColor = new Color("#b44343");
    private StyleBox barStyle;
    public override void _Ready()
    {
        barStyle = (StyleBoxFlat)healthBar.Get("theme_override_styles/fill");
    }
    public void SetPlayer(Player player)
    {
        this.player = player;
        if (player != null)
        {
            SetCurrentHealth(player.Stats.Health);
            SetMaxHealth(player.Stats.MaxHealth);
            ChangeGoldText(player.Stats.Gold);
            player.Connect("PlayerHealthChanged", new Callable(this, "SetCurrentHealth"));
            player.Connect("PLayerGoldChanged", new Callable(this, "SetCurrentGold"));
            SetWeapon(player.WeaponsManager.GetCurrentWeapon());
            animationPlayer.Play("Idle");
            player.WeaponsManager.Connect("WeaponChanged", new Callable(this, "SetWeapon"));
        }
    }
    public void SetWeapon(Weapon weapon)
    {
        if (weapon is Projective projective)
        {
            SetCurrentAmmo(projective.CurrentAmmo);
            if (!weapon.IsConnected("WeaponAmmoChanged", new Callable(this, "SetCurrentAmmo")))
            {
                weapon.Connect("WeaponAmmoChanged", new Callable(this, "SetCurrentAmmo"));
                weapon.Connect("PlayerReloaded", new Callable(this, "ReloadAnimation"));
            }
        }
        if (weapon is Melee)
        {
            SetCurrentAmmo(0);
        }
    }
    private void SetCurrentHealth(float newHealth)
    {
        healthTween = CreateTween();
        healthTween.SetEase(Tween.EaseType.In);
        healthTween.SetTrans(Tween.TransitionType.Linear);
        healthTween.SetParallel(true);
        healthTween.TweenProperty(healthBar, "value", newHealth, 0.4f);
        healthTween.TweenProperty(barStyle, "bg_color", hightlightColor, 0.2f);
        healthTween.TweenProperty(barStyle, "bg_color", originalColor, 0.2f).SetDelay(0.2f);
        currentHealth.Text = newHealth.ToString();
    }
    private void SetMaxHealth(float newMaxHealth) => maxHealth.Text = newMaxHealth.ToString();
    private void SetCurrentAmmo(int newAmmo) => currentAmmo.Text = newAmmo.ToString();
    private void SetCurrentGold(int newGold, int oldGold)
    {
        Tween goldTween = CreateTween();
        goldTween.SetTrans(Tween.TransitionType.Expo);
        goldTween.SetEase(Tween.EaseType.Out);
        goldTween.TweenMethod(new Callable(this, "ChangeGoldText"), oldGold, newGold, 0.6f);
    }
    private void ChangeGoldText(int value)
    {
        currentGold.Text = value.ToString();
        //if (value >= 100000)
        //{
        //    currentGold.Text = string.Format("{0}k", value / 1000);
        //}
        //else
        //{
        //    currentGold.Text = value.ToString();
        //}
    }
    private void ReloadAnimation(int ammo)
    {
        switch (ammo)
        {
            case 4:
                animationPlayer.Play("3 ArrowsLeft");
                break;
            case 3:
                animationPlayer.Play("2 ArrowsLeft");
                break;
            case 2:
                animationPlayer.Play("1 ArrowsLeft");
                break;
            case 1:
                animationPlayer.Play("0 ArrowsLeft");
                break;
            case 0:
                break;
            default:
                animationPlayer.Play("Default");
                break;
        }
    }
    private void CancelAttackButtonPressed()
    {
        if (player != null)
        {
            player.CancelAttack();
            animationPlayer.Play("Idle");
        }
    }
    private void SwitchWeaponPressed()
    {
        if (player != null)
        {
            player.ChangeWeapon();
        }
    }
    private void PauseButtonPressed()
    {
        var parent = GetParent();
        if (parent is Map map)
        {
            map.Pause();
        }
    }
    private void StartNewWaveButton()
    {
        EmitSignal("NewWaveStarted");
        newWaveButton.Hide();
    }
    public void ToggleNewWaveButton(bool enabled)
    {
        if (enabled)
        {
            newWaveButton.Show();
        }
        else
        {
            newWaveButton.Hide();
        }
    }
}
