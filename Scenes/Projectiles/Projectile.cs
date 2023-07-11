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

    private float traveledDistance = 0;
    private float moveAmount = 0;
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
    public void AddToScene()
    {
        SetPhysicsProcess(true);
        flyingSound.Play();
        Show();
    }
    public void RemoveFromScene()
    {
        SetPhysicsProcess(false);
        flyingSound.Stop();
        direction = Vector2.Zero;
        Position = Vector2.Zero;
        traveledDistance = 0;
        Rotation = 0;
        Hide();
        if (RemovedFromScene != null)
        {
            RemovedFromScene(this);
        }
    }
}
