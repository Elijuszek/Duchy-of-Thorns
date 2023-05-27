using System.Collections.Generic;

namespace DuchyOfThorns;

public partial class MeleeAI : Node2D
{

	[Export] private bool debugging = false;
	public enum State
	{
		PATROL,
		ENGAGE,
		ADVANCE,
		ATTACK
	}

	protected Timer patrolTimer;
	protected Line2D pathLine;
	protected Area2D detectionZone;
	protected Area2D attackZone;
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
		attackZone = GetNode<Area2D>("AttackZone");
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
						actor.Velocity = actor.VelocityToward(path[1]);
						actor.RotateToward(path[1]);
						actor.MoveAndSlide();

						// Debugging
						SetPathLine(path);
						// Debugging
					}
					else
					{
						patrolLocationReached = true;
						actor.Velocity = Vector2.Zero;
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
					Vector2[] path2 = pathfinding.GetEngagePath(GlobalPosition, target.GlobalPosition);
					if (path2.Length > 1)
					{
						actor.Velocity = actor.VelocityToward(path2[1]);
						actor.RotateToward(path2[1]);
						actor.MoveAndSlide();

						// Debugging
						SetPathLine(path2);
						// Debuging
					}
				}
				break;
			case (int)State.ATTACK:
				actor.Velocity = Vector2.Zero;
				actor.RotateToward(target.GlobalPosition);
				(actor as Infantry).Attack();

				// Debugging
				pathLine.ClearPoints();
				// Debugging
				break;
			case (int)State.ADVANCE:
				//CheckDetectionZone();
				Vector2[] path3 = pathfinding.GetNewPath(GlobalPosition, NextBase);
				if (path3.Length > 1)
				{
					actor.Velocity = actor.VelocityToward(path3[1]);
					actor.RotateToward(path3[1]);
					actor.MoveAndSlide();

					// Debugging
					SetPathLine(path3);
					// Debuging
				}
				else if (actor.HasReachedPosition(NextBase))
				{
					SetState((int)State.PATROL);

					// Debugging
					pathLine.ClearPoints();
					// Debugging
				}

				// TOTW might find closest usable tile instead of getting random one, when path is lost due to other movement
				else if (NextBaseObject != null)
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
			CallDeferred("CheckDetectionZone");
			Origin = GlobalPosition;
			actor.Velocity = Vector2.Zero; // not walking
			patrolLocationReached = true;
			patrolTimer.Start();

		}
		else if (newState == (int)State.ADVANCE)
		{
			CallDeferred("CheckDetectionZone");
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
		if (body is Actor actorBody && actorBody.GetTeam() != team &&
			CurentState != (int)State.ENGAGE && CurentState != (int)State.ATTACK)
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
	private void AttackZoneBodyEntered(Node body)
	{
		if (body is Actor actorBody && actorBody.GetTeam() != team) // body == target && target != null
		{
			target = actorBody;
			SetState((int)State.ATTACK);
		}
	}
	private void AttackZoneBodyExited(Node body)
	{
		if (target != null)
		{
			SetState((int)State.ENGAGE);
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
