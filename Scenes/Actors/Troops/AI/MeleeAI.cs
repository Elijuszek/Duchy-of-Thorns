using System.Collections.Generic;

namespace DuchyOfThorns;

/// <summary>
/// Class which controls the behaviour of melee units
/// </summary>
public partial class MeleeAI : NavigationAgent2D
{
    public enum State
    {
        PATROL,
        ENGAGE,
        ADVANCE,
        ATTACK
    }
    protected Timer patrolTimer;
    protected Area2D detectionZone;
    protected Area2D attackZone;
    public State CurrentState { get; set; }
    private CharacterBody2D enemy = null;
    private Infantry parent = null;
    private Weapon weapon = null;
    private Team.TeamName team;

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
            case State.ADVANCE:
                if (IsTargetReached())
                {
                    SetState(State.PATROL);
                    return;
                }
                parent.Direction = parent.GlobalPosition.DirectionTo(GetNextPathPosition());
                parent.Velocity += parent.Direction * parent.Stats.Speed;
                SetVelocity(parent.Velocity);
                break;

            case State.ENGAGE:
                TargetPosition = enemy.GlobalPosition;
                SetVelocity(parent.Velocity);
                break;

            case State.ATTACK:

                break;

            default:
                break;
        }
    }

    public void Initialize(Weapon weapon, Team.TeamName team, TileMap map)
    {
        this.team = team;
        this.weapon = weapon;
        SetNavigationMap(map.GetNavigationMap(0));
    }

    public void SetState(State newState)
    {
        if (newState == CurrentState)
        {
            return;
        }
        switch (newState)
        {
            case State.ENGAGE:
                patrolTimer.Stop();
                break;
            case State.ADVANCE:
                patrolTimer.Stop();
                TargetPosition = NextBase;

                break;
            case State.PATROL:
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
        SetState(State.ADVANCE);
    }

    private void DetectionZoneBodyEntered(Node body)
    {
        if (body is Actor actorBody && actorBody.GetTeam() != team &&
            CurrentState != State.ENGAGE && CurrentState != State.ATTACK)
        {
            enemy = actorBody;
            SetState(State.ENGAGE);
        }
    }

    private void DetectionZoneBodyExited(Node body)
    {
        if (body == enemy && enemy != null && !IsQueuedForDeletion())
        {
            SetState(State.ADVANCE);
            enemy = null;
        }
    }

    private void AttackZoneBodyEntered(Node body)
    {
        if (body is Actor actorBody && actorBody.GetTeam() != team) // body == enemy && enemy != null
        {
            enemy = actorBody;
            SetState(State.ATTACK);
        }
    }

    private void AttackZoneBodyExited(Node body)
    {
        if (enemy != null)
        {
            SetState(State.ENGAGE);
        }
    }
}
