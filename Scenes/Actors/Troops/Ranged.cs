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
    [Export] protected Projective weapon;
    [Export] protected Timer patrolTimer;
    [Export] protected Area2D detectionZone;
    [Export] private NavigationAgent2D navAgent;


    private Actor enemy = null;
    private Rid navigationMap;

    public override void _Ready()
    {
        base._Ready();

        // Navigation
        navigationMap = GetNode<NavigationRegion2D>("/root/World/NavigationRegion2D").GetNavigationMap();
        navAgent.SetNavigationMap(navigationMap);
        navAgent.Connect("velocity_computed", new Callable(this, "Walking"));
        navAgent.MaxSpeed = Stats.Speed;
        navAgent.TargetPosition = GlobalPosition;

        // Other
        weapon.Initialize(Team);
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        switch (CurrentState)
        {
            case TroopState.PATROL:
                Idle();
                Velocity = knockback;
                return;

            case TroopState.ADVANCE:
                if (navAgent.IsTargetReached())
                {
                    SetState(TroopState.PATROL);
                    return;
                }
                Direction = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition());
                navAgent.Velocity = Velocity + Direction * Stats.Speed + knockback; // Emmits signal velocity_computed
                RotateToward(Velocity.Angle());
                break;

            case TroopState.ATTACK:
                RotateToward(enemy.GlobalPosition);
                Attack();
                Velocity = knockback;
                break;

            case TroopState.ENGAGE:
                GD.PrintErr("RangedAI ENGAGE state is not implemented");
                break;

            default:
                GD.PrintErr("Invalid TroopState in Ranged");
                break;
        }
        MoveAndSlide();
    }

    public override void SetState(TroopState newState)
    {
        base.SetState(newState);
        switch (newState)
        {
            case TroopState.ADVANCE:
                patrolTimer.Stop();
                navAgent.TargetPosition = Destination;
                navAgent.AvoidanceEnabled = true;
                RefreshDetectionZone();
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
        Velocity = velocity + knockback;
    }
    private void Attack() 
    { 
        if (weapon.CanAttack())
        {
            weapon.Attack();
            float customSpeed = animationPlayer.GetAnimation("Attack").Length / weapon.AttackDuartion;
            animationPlayer.Play("Attack", -1, customSpeed);
        }
    }
    private void Deliver()
    {
        weapon.Deliver();
        animationPlayer.Play("Idle");
    }
    private void RefreshDetectionZone()
    {
        detectionZone.SetDeferred("monitoring", false);
        detectionZone.SetDeferred("monitoring", true);
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
            RefreshDetectionZone();
        }
    }

    private void PatrolTimerTimeout()
    {
        float patrolRange = 50f;
        float randomX = Utilities.GetRandomFloat(-patrolRange, patrolRange);
        float randomY = Utilities.GetRandomFloat(-patrolRange, patrolRange);
        Destination = NavigationServer2D.MapGetClosestPoint(navigationMap, new Vector2(randomX, randomY) + Origin);
        SetState(TroopState.ADVANCE);
    }
}
