using DuchyofThorns.Scenes.Globals;

namespace DuchyOfThorns;

/// <summary>
/// Intermediate class for all infantry units
/// </summary>
public partial class Infantry : Troop
{
    public TroopState CurrentState { get; set; }

    protected Timer patrolTimer;
    protected Area2D detectionZone;
    protected Area2D attackZone;
    protected Timer attackTimer;
    protected NavigationAgent2D navAgent;

    private CharacterBody2D enemy = null;
    private Weapon weapon = null;

    public Vector2 NextBase { get; set; } = Vector2.Zero;
    public override void _Ready()
	{
		base._Ready();
        patrolTimer = GetNode<Timer>("PatrolTimer");
        detectionZone = GetNode<Area2D>("DetectionZone");
        attackZone = GetNode<Area2D>("AttackZone");
        attackTimer = GetNode<Timer>("AttackTimer");
        navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

        navAgent.MaxSpeed = Stats.Speed;
        navAgent.SetNavigationMap(GetNode<TileMap>("/root/World/TileMap").GetNavigationMap(0));
        navAgent.TargetPosition = GlobalPosition;
        navAgent.Connect("velocity_computed", new Callable(this, "Move"));
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        switch (CurrentState)
        {
            case TroopState.ADVANCE:
                if (navAgent.IsTargetReached())
                {
                    SetState(TroopState.PATROL);
                    return;
                }
                Direction = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition());
                Velocity += Direction * Stats.Speed;
                navAgent.SetVelocity(Velocity);
                break;

            case TroopState.ENGAGE:
                navAgent.TargetPosition = enemy.GlobalPosition;
                navAgent.SetVelocity(Velocity);
                break;

            case TroopState.ATTACK:

                break;

            default:
                break;
        }
    }

    public void SetState(TroopState newState)
    {
        if (newState == CurrentState)
        {
            return;
        }
        switch (newState)
        {
            case TroopState.ENGAGE:
                patrolTimer.Stop();
                break;
            case TroopState.ADVANCE:
                patrolTimer.Stop();
                navAgent.TargetPosition = NextBase;

                break;
            case TroopState.PATROL:
                Velocity = Vector2.Zero;
                patrolTimer.Start();
                break;
        }
        CurrentState = newState;
    }

    private void Move(Vector2 velocity)
    {
        Velocity = velocity;
        Rotation = Mathf.Lerp(Rotation, Velocity.Angle(), 0.1f);
        MoveAndSlide();
    }

    private void DetectionZoneBodyEntered(Node2D body)
    {
        if (body is Actor actorBody && actorBody.GetTeam() != Team &&
            CurrentState != TroopState.ENGAGE && CurrentState != TroopState.ATTACK)
        {
            enemy = actorBody;
            SetState(TroopState.ENGAGE);
        }
    }

    private void DetectionZoneBodyExited(Node2D body)
    {
        if (body == enemy && enemy != null && !IsQueuedForDeletion())
        {
            SetState(TroopState.ADVANCE);
            enemy = null;
        }
    }

    private void AttackZoneBodyEntered(Node2D body)
    {
        if (body is Actor actorBody && actorBody.GetTeam() != Team) // body == enemy && enemy != null
        {
            enemy = actorBody;
            SetState(TroopState.ATTACK);
        }
    }

    private void AttackZoneBodyExited(Node2D body)
    {
        if (enemy != null)
        {
            SetState(TroopState.ENGAGE);
        }
    }

    private void PatrolTimerTimeout()
    {
        float patrolRange = 150;
        float randomX = Globals.GetRandomFloat(-patrolRange, patrolRange);
        float randomY = Globals.GetRandomFloat(-patrolRange, patrolRange);
        NextBase = new Vector2(randomX, randomY) + GlobalPosition;
        SetState(TroopState.ADVANCE);
    }

    protected virtual void AttackTimerTimeout() => GD.PrintErr("Calling AttackTimerTimeout from Infantry class");
    public virtual void Attack() => GD.PrintErr("Calling Attack from Infantry class");
}
