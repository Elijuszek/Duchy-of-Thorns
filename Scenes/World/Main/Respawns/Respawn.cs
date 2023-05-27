namespace DuchyOfThorns;

/// <summary>
/// Base class for all respawn points
/// </summary>
public partial class Respawn : Marker2D
{
    [Signal] public delegate void OutOfTroopsEventHandler();
    [Export] public float RespawnCooldown { get; set; } = 5;
    [Export] public int RespawnCount { get; set; } = 5;
    [Export] public PackedScene Unit { get; set; } = null;
    protected Pathfinding pathfinding;
    private Timer respawnTimer;

    public override void _Ready()
    {
        base._Ready();
        respawnTimer = GetNode<Timer>("RespawnTimer");
        respawnTimer.WaitTime = RespawnCooldown;
    }
    public void Initialize(Pathfinding pathfinding)
    {
        this.pathfinding = pathfinding;
    }
    public void SetUnit(PackedScene toSet, float cooldown = 5, int count = 5)
    {
        this.Unit = toSet;
        this.RespawnCooldown = cooldown;
        this.RespawnCount = count;
    }
    protected Vector2 GetRandomPositionWithinRadius()
    {
        float x = GetRandomPosition(GlobalPosition.X - 25, GlobalPosition.X + 25);
        float y = GetRandomPosition(GlobalPosition.Y - 25, GlobalPosition.Y + 25);
        return new Vector2(x, y);
    }
    private float GetRandomPosition(float min, float max)
    {
        Random rand = new Random();
        float range = max - min;
        double sample = rand.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }
    protected void HandleUnitDeath() => respawnTimer.Start();
    protected void RespawnTimerTimeout() => SpawnUnit();
    public virtual void Clear()
    {
        Unit = null;
        RespawnCount = -1;
        RespawnCooldown = -1;
    }

    public virtual void SpawnUnit() => GD.PrintErr("Calling SpawnUnit from Respawn class");

    public virtual void SetCapturableBase(CapturableBase nextBase, Vector2 nextBaseCord) => GD.PrintErr("Calling SetCapturableBase from Respawn class");
}
