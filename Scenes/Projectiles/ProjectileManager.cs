namespace DuchyOfThorns;

/// <summary>
/// Projectile manager is dedicated to handle fired and removed projectiles in the scene
/// It also increases performance by using object pooling (projectiles will be instantiated only ounce)
/// Projectile manager mustn't be blocked by any other body in the scene, because collision 
/// will trigger unwanted arrowRemoved event
/// </summary>
public partial class ProjectileManager : Node
{
    [Export] private int fireArrowsCount = 10;
    [Export] private int arrowsCount = 20;
    private PackedScene arrowScene;
    private PackedScene fireArrowScene;
    private ObjectPool<Arrow> arrowPool;
    private ObjectPool<FireArrow> fireArrowPool;
    public override void _Ready()
    {
        base._Ready();
        arrowScene = (PackedScene)ResourceLoader.Load("res://Scenes/Projectiles/Arrows/Arrow.tscn");
        fireArrowScene = (PackedScene)ResourceLoader.Load("res://Scenes/Projectiles/Arrows/FireArrow.tscn");
        arrowPool = new ObjectPool<Arrow>();
        fireArrowPool = new ObjectPool<FireArrow>();
        for (int i = 0; i < arrowsCount; i++)
        {
            Arrow arrow = arrowScene.Instantiate() as Arrow;
            arrow.Connect("ProjectileRemoved", new Callable(this, "ArrowRemoved"));
            AddChild(arrow);
            arrow.RemoveFromScene();
        }
        for (int i = 0; i < fireArrowsCount; i++)
        {
            FireArrow arrow = fireArrowScene.Instantiate() as FireArrow;
            arrow.Connect("ProjectileRemoved", new Callable(this, "FireArrowRemoved"));
            AddChild(arrow);
            arrow.RemoveFromScene();
        }
    }
    public void HandleArrowSpawned(Arrow arrow, int team, Vector2 position, Vector2 direction)
    {
        float damage = arrow.Damage;
        if (arrow is FireArrow)
        {
            arrow = fireArrowPool.Get();
            (arrow as FireArrow).StartEmiting();
        }
        else
        {
            arrow = arrowPool.Get();
        }
        arrow.Damage = damage;
        arrow.AddToScene();
        arrow.GlobalPosition = position;
        arrow.team = team;
        arrow.SetDirection(direction);
    }
    private void ArrowRemoved(Arrow removed)
    {
        arrowPool.Release(removed);
    }
    private void FireArrowRemoved(FireArrow removed)
    {
        fireArrowPool.Release(removed);
        removed.StopEmiting();

    }
}
