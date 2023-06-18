using DuchyofThorns.Scenes.Globals;
using System.Collections.Generic;

namespace DuchyOfThorns;

/// <summary>
/// Class which controls the behaviour of melee units
/// </summary>
public partial class MeleeAI : NavigationAgent2D
{
    protected Timer patrolTimer;
    protected Area2D detectionZone;
    protected Area2D attackZone;
    public TroopState CurrentState { get; set; }
    private CharacterBody2D enemy = null;
    private Infantry parent = null;
    private Weapon weapon = null;
    private Team team;

    //ADVANCE STATE
    public Vector2 NextBase { get; set; } = Vector2.Zero;
    public override void _Ready()
    {
        patrolTimer = GetNode<Timer>("PatrolTimer");
        detectionZone = GetNode<Area2D>("DetectionZone");
        attackZone = GetNode<Area2D>("AttackZone");
        parent = GetParent<Infantry>();

        TargetPosition = parent.GlobalPosition;
        Connect("velocity_computed", new Callable(this, "Move"));
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        switch (CurrentState)
        {
            case TroopState.ADVANCE:
                if (IsTargetReached())
                {
                    SetState(TroopState.PATROL);
                    return;
                }
                parent.Direction = parent.GlobalPosition.DirectionTo(GetNextPathPosition());
                parent.Velocity += parent.Direction * parent.Stats.Speed;
                SetVelocity(parent.Velocity);
                break;

            case TroopState.ENGAGE:
                TargetPosition = enemy.GlobalPosition;
                SetVelocity(parent.Velocity);
                break;

            case TroopState.ATTACK:

                break;

            default:
                break;
        }
    }

    public void Initialize(Weapon weapon, Team team, TileMap map)
    {
        this.team = team;
        this.weapon = weapon;
        SetNavigationMap(map.GetNavigationMap(0));
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
                TargetPosition = NextBase;

                break;
            case TroopState.PATROL:
                parent.Velocity = Vector2.Zero;
                patrolTimer.Start();
                break;
        }
        CurrentState = newState;
    }

    private void Move(Vector2 velocity)
    {
        parent.Velocity = velocity;
        parent.Rotation = Mathf.Lerp(parent.Rotation, parent.Velocity.Angle(), 0.1f);
        parent.MoveAndSlide();
    }

    private void PatrolTimerTimeout()
    {
        float patrolRange = 150;
        float randomX = Globals.GetRandomFloat(-patrolRange, patrolRange);
        float randomY = Globals.GetRandomFloat(-patrolRange, patrolRange);
        NextBase = new Vector2(randomX, randomY) + parent.GlobalPosition;
        SetState(TroopState.ADVANCE);
    }

    private void DetectionZoneBodyEntered(Node body)
    {
        if (body is Actor actorBody && actorBody.GetTeam() != team &&
            CurrentState != TroopState.ENGAGE && CurrentState != TroopState.ATTACK)
        {
            enemy = actorBody;
            SetState(TroopState.ENGAGE);
        }
    }

    private void DetectionZoneBodyExited(Node body)
    {
        if (body == enemy && enemy != null && !IsQueuedForDeletion())
        {
            SetState(TroopState.ADVANCE);
            enemy = null;
        }
    }

    private void AttackZoneBodyEntered(Node body)
    {
        if (body is Actor actorBody && actorBody.GetTeam() != team) // body == enemy && enemy != null
        {
            enemy = actorBody;
            SetState(TroopState.ATTACK);
        }
    }

    private void AttackZoneBodyExited(Node body)
    {
        if (enemy != null)
        {
            SetState(TroopState.ENGAGE);
        }
    }
}
