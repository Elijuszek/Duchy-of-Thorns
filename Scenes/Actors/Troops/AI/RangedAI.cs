using System.Collections.Generic;

namespace DuchyOfThorns;

/// <summary>
///  Class which controls the behaviour of ranged units
/// </summary>
public partial class RangedAI : Node2D
{

    [Export] private bool debugging = false;
    public enum State
    {
        PATROL,
        ENGAGE,
        ADVANCE
    }

    protected Timer patrolTimer;
    protected Line2D pathLine;
    protected Area2D detectionZone;
    public int CurentState { get; set; }
    private Actor actor = null;
    private CharacterBody2D target = null;
    private Weapon weapon = null;
    private int team = -1;

    // PATROL STATE
    public Vector2 Origin { get; set; } = Vector2.Zero;
    private Vector2 patrolLocation = Vector2.Zero;
    public Vector2 Velocity { get; set; } = Vector2.Zero;
    private bool patrolLocationReached = false;

    //ADVANCE STATE
    public CapturableBase NextBaseObject { get; set; } = null;
    public Vector2 NextBase { get; set; } = Vector2.Zero;

    public Pathfinding pathfinding { get; set; }
    public override void _Ready()
    {
        patrolTimer = GetNode<Timer>("PatrolTimer");
        pathLine = GetNode<Line2D>("PathLine");
        detectionZone = GetNode<Area2D>("DetectionZone");
        patrolLocation = GlobalPosition;
        SetState((int)State.PATROL);
        // Debugging
        pathLine.Visible = debugging;
        // Debugging
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        pathLine.GlobalRotation = 0;

        switch (CurentState)
        {
            case (int)State.PATROL:
                if (!patrolLocationReached)
                {
                    Vector2[] path = pathfinding.GetNewPath(GlobalPosition, patrolLocation);
                    if (path.Length > 1)
                    {
                        Velocity = actor.VelocityToward(path[1]);
                        actor.RotateToward(path[1]);
                        actor.Velocity = Velocity;

                        // Debugging
                        SetPathLine(path);
                        // Debugging
                    }
                    else
                    {
                        patrolLocationReached = true;
                        Velocity = Vector2.Zero;
                        actor.Velocity = Velocity;
                        patrolTimer.Start();

                        // Debugging
                        pathLine.ClearPoints();
                        // Debugging
                    }
                }
                break;
            case (int)State.ENGAGE:
                if (target != null && weapon != null)
                {
                    Velocity = Vector2.Zero;
                    actor.Velocity = Velocity;
                    actor.RotateToward(target.GlobalPosition);
                    if (Math.Abs(actor.GetAngleTo(target.GlobalPosition)) < 0.1)
                    {
                        (actor as Ranged).Attack();
                    }
                }
                break;
            case (int)State.ADVANCE:
                CheckDetectionZone();
                Vector2[] path2 = pathfinding.GetNewPath(GlobalPosition, NextBase);
                if (path2.Length > 1)
                {
                    Velocity = actor.VelocityToward(path2[1]);
                    actor.RotateToward(path2[1]);
                    actor.Velocity = Velocity;
                    // Debugging
                    SetPathLine(path2);
                    // Debuging
                }
                else if (actor.HasReachedPosition(NextBase))
                {
                    SetState((int)State.PATROL);

                    // Debugging
                    pathLine.ClearPoints();
                    // Debugging
                }
                else if (NextBaseObject != null) // TODO fix for guard modem, might loose path
                {
                    NextBase = NextBaseObject.GetRandomPositionWithinRadius();
                }
                break;
        }
    }
    public void Initialize(CharacterBody2D actor, Weapon weapon, int team)
    {
        this.actor = actor as Actor;
        this.team = team;
        this.weapon = weapon;
    }
    public void SetState(int newState)
    {
        if (newState == CurentState)
        {
            return;
        }
        if (newState == (int)State.PATROL)
        {
            Origin = GlobalPosition;
            Velocity = Vector2.Zero; // not walking
            patrolLocationReached = true;
            patrolTimer.Start();

        }
        else if (newState == (int)State.ADVANCE)
        {
            if (actor.HasReachedPosition(NextBase))
            {
                SetState((int)State.PATROL);
                return;
            }
        }
        CurentState = newState;
    }
    // Debugging
    private void SetPathLine(Vector2[] points)
    {
        if (!debugging)
        {
            return;
        }
        List<Vector2> localPoints = new List<Vector2>();
        foreach (Vector2 point in points)
        {
            if (point == points[0])
            {
                localPoints.Add(Vector2.Zero);
            }
            else
            {
                localPoints.Add(point - GlobalPosition);
            }
        }
        pathLine.Points = localPoints.ToArray();
    }
    // Debugging
    private void CheckDetectionZone()
    {
        detectionZone.Monitoring = false;
        detectionZone.Monitoring = true;
    }
    private void PatrolTimerTimeout()
    {
        float patrolRange = 75;
        float randomX = GetRandomFloat(-patrolRange, patrolRange);
        float randomY = GetRandomFloat(-patrolRange, patrolRange);
        patrolLocation = new Vector2(randomX, randomY) + Origin;
        patrolLocationReached = false;
    }
    private void DetectionZoneBodyEntered(Node body)
    {
        if (body is Actor actorBody && actorBody.GetTeam() != team && CurentState != (int)State.ENGAGE)
        {
            SetState((int)State.ENGAGE);
            target = actorBody;
        }
    }
    private void DetectionZoneBodyExited(Node body)
    {
        if (body == target && target != null && !IsQueuedForDeletion())
        {
            SetState((int)State.ADVANCE);
            target = null;
        }
    }
    private float GetRandomFloat(float min, float max)
    {
        Random rand = new Random();
        double range = max - min;
        double sample = rand.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }

}
