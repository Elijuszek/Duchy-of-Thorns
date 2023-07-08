namespace DuchyOfThorns;

/// <summary>
/// Final class for all infantry units
/// Main purpose of this class is to implement specific
/// logic and functionality for every troop state
/// </summary>
public partial class Infantry : Troop
{
    public TroopState CurrentState { get; set; }
    public Vector2 AdvancePosition { get; set; }

    [Export] protected Melee weapon;

    [Export] protected AnimationPlayer animationPlayer;
    [Export] protected Timer patrolTimer;
    [Export] protected Area2D detectionZone;
    [Export] protected Area2D attackZone;

    [Export] private NavigationAgent2D navAgent;
    private Actor enemy = null;

    public override void _Ready()
    {
        base._Ready();
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
            case TroopState.PATROL:
                return;

            case TroopState.ADVANCE:
                if (navAgent.IsTargetReached())
                {
                    SetState(TroopState.PATROL);
                    return;
                }
                Direction = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition());
                navAgent.Velocity = Velocity + Direction * Stats.Speed;
                RotateToward(Velocity.Angle());
                break;

            case TroopState.ENGAGE:
                navAgent.TargetPosition = enemy.GlobalPosition; // Enemy position is dynamic
                Direction = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition());
                navAgent.Velocity = Velocity + Direction * Stats.Speed;
                RotateToward(Velocity.Angle());
                break;

            case TroopState.ATTACK:
                RotateToward(enemy.GlobalPosition);
                //Attack();
                break;

            default:
                GD.PushError("Invalid TroopState");
                break;
        }
    }

    public void SetState(TroopState newState)
    {
        if (newState == CurrentState)
            return;
        switch (newState)
        {
            case TroopState.ADVANCE:
                patrolTimer.Stop();
                navAgent.TargetPosition = AdvancePosition; // Advance position is static
                navAgent.AvoidanceEnabled = true;
                break;

            case TroopState.PATROL:
                patrolTimer.Start();
                Velocity = Vector2.Zero;
                navAgent.AvoidanceEnabled = false;
                break;

            case TroopState.ENGAGE:
                patrolTimer.Stop();
                navAgent.AvoidanceEnabled = true;
                break;

            case TroopState.ATTACK:
                patrolTimer.Stop();
                navAgent.AvoidanceEnabled = false;
                Velocity = Vector2.Zero;
                break;
        }
        CurrentState = newState;
    }

    private void Move(Vector2 velocity)
    {
        Velocity = velocity;
        MoveAndSlide();
    }

    private void DetectionZoneBodyEntered(PhysicsBody2D body)
    {
        if (body is Actor actorBody && actorBody.GetTeam() != Team &&
            CurrentState != TroopState.ENGAGE && CurrentState != TroopState.ATTACK)
        {
            enemy = actorBody;
            SetState(TroopState.ENGAGE);
        }
    }

    private void DetectionZoneBodyExited(PhysicsBody2D body)
    {
        if (body == enemy && enemy != null && !IsQueuedForDeletion())
        {
            SetState(TroopState.ADVANCE);
            enemy = null;
        }
    }

    private void AttackZoneBodyEntered(PhysicsBody2D body)
    {
        if (body is Actor actorBody && actorBody.GetTeam() != Team)
        {
            enemy = actorBody;
            SetState(TroopState.ATTACK);
        }
    }

    private void AttackZoneBodyExited(PhysicsBody2D body)
    {
        if (enemy is not null)
        {
            SetState(TroopState.ENGAGE);
        }
    }

    private void PatrolTimerTimeout()
    {
        float patrolRange = 150f;
        float randomX = Globals.GetRandomFloat(-patrolRange, patrolRange);
        float randomY = Globals.GetRandomFloat(-patrolRange, patrolRange);
        AdvancePosition = new Vector2(randomX, randomY) + AdvancePosition;
        SetState(TroopState.ADVANCE);
    }
}
