using Godot;

public partial class Stats : Node2D
{
    [Export] public float Health { get; set; } = 100;
    [Export] public float MaxHealth { get; set; } = 100;
    [Export] public float DamageMultiplier { get; set; } = 1;
    [Export] public float Armour { get; set; } = 5;
    [Export] public float Speed { get; set; }
    [Export] public int Gold { get; set; } = 0;
    public void SetHealth(float newHealth)
    {
        Health = Mathf.Clamp(newHealth, 0, MaxHealth);
    }
    public void SetMaxHealth(float newMaxHealth)
    {
        MaxHealth = newMaxHealth;
    }
}
