namespace DuchyOfThorns;

/// <summary>
/// Base class for all projectiles
/// </summary>
public partial class Projectile : Area2D, IPoolable
{
    public event RemovedFromSceneEventHandler RemovedFromScene;
    [Export] public ProjectileType Type { get; set; }
    [Export] public float Speed { get; set; } = 4;
    [Export] public float Damage { get; set; } = 35;
    [Export] public float Range { get; set; } = 10;
    [Export] protected AudioStreamPlayer2D flyingSound;
    [Export] protected CollisionShape2D collisionShape;
    protected float traveledDistance = 0;
    protected float moveAmount = 0;
    protected Vector2 direction = Vector2.Zero;
    
    public Team team { get; set; }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (direction != Vector2.Zero)
        {
            Vector2 velocity = direction * Speed;
            GlobalPosition += velocity;
            moveAmount = Convert.ToSingle(delta) * Speed;
            traveledDistance += moveAmount;
            if (traveledDistance > Range)
            {
                RemoveFromScene();
            }
        }
    }
    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
        Rotation = direction.Angle();
    }
    public virtual void AddToScene()
    {
        collisionShape.SetDeferred("disabled", false);
        flyingSound.Play();
        Show();
        SetPhysicsProcess(true);
    }
    public virtual void RemoveFromScene()
    {
        Hide();
        flyingSound.Stop();
        direction = Vector2.Zero;
        GlobalPosition = Vector2.Zero;
        traveledDistance = 0;
        Rotation = 0;
        collisionShape.SetDeferred("disabled", true);
        SetPhysicsProcess(false);
        if (RemovedFromScene != null)
        {
            RemovedFromScene(this);
        }
    }
}
