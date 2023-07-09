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
    [Export] private Area2D takeArea;
    [Export] private Area2D slideArea;

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
        Show();
    }
    public void RemoveFromScene()
    {
        Hide();
        if (tween != null) // TODO
        {
            tween.Stop();
        }
        timer.Stop();
        GlobalPosition = Vector2.Zero;
        movementDirection = GlobalPosition;
        if (RemovedFromScene != null)
        {
            RemovedFromScene(this);
        }

        //EmitSignal("RemovedFromScene", this);

    }
}