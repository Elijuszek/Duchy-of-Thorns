namespace DuchyOfThorns;

/// <summary>
/// TODO: should inherit from loot class
/// Class for coin functionality
/// </summary>
public partial class Coin : CharacterBody2D, IPoolable
{
    [Signal] public delegate void CoinRemovedEventHandler(Coin coin);
    [Export] public int Gold { get; set; } = 0;
    [Export] private Timer timer;
    [Export] private Area2D takeArea;
    [Export] private Area2D slideArea;

    private Vector2 movementDirection = Vector2.Zero;
    private Tween tween;
    
    private void Move()
    {
        tween = CreateTween();
        tween.TweenProperty(this, "global_position", movementDirection, 0.2f);
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
            Move();
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
        movementDirection += GlobalPosition;
        timer.Start();
        Move();
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
        EmitSignal(nameof(CoinRemoved), this);
    }
}