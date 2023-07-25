namespace DuchyOfThorns;

/// <summary>
/// Final class for all infantry units
/// Main purpose of this class is to implement specific
/// logic and functionality for every troop state
/// </summary>
public partial class Infantry : Troop
{
    [Export] protected Melee weapon;
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
        navAgent.Connect("velocity_computed", new Callable(this, "Walking"));
        weapon.Initialize(Team);
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        switch (CurrentState)
        {
            case TroopState.PATROL:
                Idle();
                break;

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
                Attack();
                break;

            default:
                GD.PushError("Invalid TroopState");
                break;
        }
    }

    public override void SetState(TroopState newState)
    {
        base .SetState(newState);
        switch (newState)
        {
            case TroopState.ADVANCE:
                patrolTimer.Stop();
                navAgent.TargetPosition = Destination; // Advance position is static
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
    private void Idle()
    {
        weapon.Idle();
        animationPlayer.Play("Idle");
    }
    private void Walking(Vector2 velocity)
    {
        weapon.Walking();
        animationPlayer.Play("Walk");
        Velocity = velocity;
        MoveAndSlide();
    }
    private void Attack() 
    {
        if (animationPlayer.CurrentAnimation != "Attack" && weapon.CanAttack())
        {
            weapon.Attack();
            float customSpeed = animationPlayer.GetAnimation("Attack").Length / weapon.AttackDuartion;
            animationPlayer.Play("Attack", -1, customSpeed);
        }
    }

    private void RefreshDetectionZone()
    {
        detectionZone.SetDeferred("monitoring", false);
        detectionZone.SetDeferred("monitoring", true);
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
        if (body == enemy && !IsQueuedForDeletion())
        {
            SetState(TroopState.ADVANCE);
            enemy = null;
            RefreshDetectionZone();
            return;
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
        if (body == enemy && !IsQueuedForDeletion())
        {
            SetState(TroopState.ENGAGE);
            return;
        }
    }

    private void PatrolTimerTimeout()
    {
        float patrolRange = 50f;
        float randomX = Utilities.GetRandomFloat(-patrolRange, patrolRange);
        float randomY = Utilities.GetRandomFloat(-patrolRange, patrolRange);
        Destination = new Vector2(randomX, randomY) + Origin;
        SetState(TroopState.ADVANCE);
    }
}
