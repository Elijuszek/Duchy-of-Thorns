namespace DuchyOfThorns;

/// <summary>
/// TODO: 
/// 1. Implement raycasting to check if there is a wall between the unit and the target,
/// if so the unit should move to a position where it can attack the target (engage state).
/// 2. Unit should cancel the attack if the target is out of range or dead.
/// 
/// Intermediate class for all ranged units
/// </summary>
public partial class Ranged : Troop
{
    public TroopState CurrentState { get; set; }
    public Vector2 AdvancePosition { get; set; }

    protected Timer patrolTimer;
    protected Timer attackTimer;
    protected Area2D detectionZone;

    private NavigationAgent2D navAgent;
    private Actor enemy = null;

    public override void _Ready()
    {
        base._Ready();
        patrolTimer = GetNode<Timer>("PatrolTimer");
        attackTimer = GetNode<Timer>("AttackTimer");

        detectionZone = GetNode<Area2D>("DetectionZone");

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
            case TroopState.PATROL:
                return;

            case TroopState.ADVANCE:
                if (navAgent.IsTargetReached())
                {
                    SetState(TroopState.PATROL);
                    return;
                }
                Direction = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition());
                navAgent.Velocity = Velocity + Direction * Stats.Speed; // Emmits signal velocity_computed
                RotateToward(Velocity.Angle());
                break;

            case TroopState.ATTACK:
                RotateToward(enemy.GlobalPosition);
                Attack();
                break;

            case TroopState.ENGAGE:
                GD.PrintErr("RangedAI ENGAGE state is not implemented");
                break;

            default:
                GD.PrintErr("Invalid TroopState");
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
            case TroopState.ADVANCE:
                patrolTimer.Stop();
                navAgent.TargetPosition = AdvancePosition;
                navAgent.AvoidanceEnabled = true;
                break;

            case TroopState.PATROL:
                patrolTimer.Start();
                navAgent.AvoidanceEnabled = false;
                Velocity = Vector2.Zero;
                break;

            case TroopState.ENGAGE:
                GD.PrintErr("RangedAI ENGAGE state is not implemented");
                return;

            case TroopState.ATTACK:
                patrolTimer.Stop();
                attackTimer.Start();
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
            SetState(TroopState.ATTACK);
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

    private void PatrolTimerTimeout()
    {
        float patrolRange = 400f;
        float randomX = Globals.GetRandomFloat(-patrolRange, patrolRange);
        float randomY = Globals.GetRandomFloat(-patrolRange, patrolRange);
        AdvancePosition = new Vector2(randomX, randomY) + AdvancePosition;
        SetState(TroopState.ADVANCE);
    }
    public virtual void Attack() => GD.PrintErr("Calling Attack from Ranged class");
    protected virtual void AttackTimerTimeout() => GD.PrintErr("Calling AttackTimerTimeout from Ranged class");
}
