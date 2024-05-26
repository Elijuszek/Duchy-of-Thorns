namespace DuchyOfThorns;

/// <summary>
/// TODO: should inherit from loot class
/// Class for coin functionality
/// </summary>
public partial class Coin : CharacterBody2D, IPoolable
{
    public event RemovedFromSceneEventHandler RemovedFromScene;
    [Export] public int Gold { get; set; } = 0;
    [Export] private Timer timer;
    [Export] private CollisionShape2D takeArea;
    [Export] private CollisionShape2D slideArea;
    [Export] private CollisionShape2D collisionShape;

    private Vector2 movementDirection = Vector2.Zero;
    private Tween tween;

    public void Move(Vector2 destination)
    {
        movementDirection += GlobalPosition;
        tween = CreateTween();
        tween.TweenProperty(this, "global_position", destination, 0.2f);
    }
    private void TimerTimeout() => RemoveFromScene();
    private void Area2DBodyEntered(Node body)
    {
        if (body is Player player)
        {
            player.GetGold(Gold);
            RemoveFromScene();
        }
    }
    private void Area2DSlideBodyEntered(Node body)
    {
        if (body is Player player)
        {
            movementDirection = player.GlobalPosition;
            Move(movementDirection);
        }
    }
    private void Area2DSlideBodyExited(Node body)
    {
        if (body is Player)
        {
            movementDirection = GlobalPosition;
            tween.Stop();
        }
    }
    public void AddToScene()
    {
        timer.Start();
        collisionShape.SetDeferred("disabled", false);
        slideArea.SetDeferred("disabled", false);
        takeArea.SetDeferred("disabled", false);
        Show();
    }
    public void RemoveFromScene()
    {
        Hide();
        timer.Stop();
        GlobalPosition = Vector2.Zero;
        movementDirection = GlobalPosition;
        collisionShape.SetDeferred("disabled", true);
        slideArea.SetDeferred("disabled", true);
        takeArea.SetDeferred("disabled", true);
        if (RemovedFromScene != null)
        {
            RemovedFromScene(this);
        }
    }
}