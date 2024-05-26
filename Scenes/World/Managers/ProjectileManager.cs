using System.Collections.Generic;

namespace DuchyOfThorns;

/// <summary>
/// Projectile manager is dedicated to handle fired and removed projectiles in the scene
/// It also increases performance by using object pooling (projectiles will be instantiated only ounce)
/// Projectile manager mustn't be blocked by any other body in the scene, because collision 
/// will trigger unwanted arrowRemoved event
/// </summary>
public partial class ProjectileManager : Node2D
{
    [Export] private int startingCount = 10;

    private Dictionary<ProjectileType, ObjectPool<Projectile>> projectilePool;

    public override void _Ready()
    {
        base._Ready();
        projectilePool = new Dictionary<ProjectileType, ObjectPool<Projectile>>()
        {
            { ProjectileType.ARROW,  new ObjectPool<Projectile>(this, 
            ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Arrows/Arrow.tscn"), startingCount)},
            { ProjectileType.FIRE_ARROW,  new ObjectPool<Projectile>(this, 
            ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Arrows/FireArrow.tscn"), startingCount)},
            { ProjectileType.FIRE_SPELL,  new ObjectPool<Projectile>(this,
            ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Spells/FireSpell.tscn"), startingCount)}
        };
    }

    public void HandleProjectileFired(ProjectileType type, float damage, Team team, Vector2 position, Vector2 direction)
    {
        Projectile projectile = projectilePool[type].Take();
        projectile.GlobalPosition = position;
        projectile.Damage = damage;
        projectile.team = team;
        projectile.SetDirection(direction);
    }
}
