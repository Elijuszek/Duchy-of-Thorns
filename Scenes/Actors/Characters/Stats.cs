namespace DuchyOfThorns;

/// <summary>
/// Class for basic storing stats of an actor
/// </summary>
[GlobalClass]
public partial class Stats : Resource
{
    [Export] public float Health { get; set; } = 100;
    [Export] public float MaxHealth { get; set; } = 100;
    [Export] public float DamageMultiplier { get; set; } = 1;
    [Export] public float Armour { get; set; } = 5;
    [Export] public float Speed { get; set; }
    [Export] public int Gold { get; set; } = 0;
    public void SetHealth(float newHealth) => Health = Mathf.Clamp(newHealth, 0, MaxHealth);
    public void SetMaxHealth(float newMaxHealth) => MaxHealth = Mathf.Clamp(newMaxHealth, 1, float.MaxValue);

    public Stats Duplicate()
    {
        return new Stats
        {
            Health = Health,
            MaxHealth = MaxHealth,
            DamageMultiplier = DamageMultiplier,
            Armour = Armour,
            Speed = Speed,
            Gold = Gold
        };
    }
}
