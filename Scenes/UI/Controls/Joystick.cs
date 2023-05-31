namespace DuchyOfThorns;

/// <summary>
/// Class for receiving player movement or attack input
/// </summary>
public partial class Joystick : TouchScreenButton
{
	private Vector2 radius = new Vector2(270, 270);
	private int boundary = 540;
	private int return_accel = 20;
	private int threshold = 10;
	public int OngoingDrag { get; set; } = -1;
	public override void _PhysicsProcess(double delta)
	{
		if (OngoingDrag == -1)
		{
			Vector2 pos_difference = Vector2.Zero - radius - Position;
			Position += pos_difference * return_accel * Convert.ToSingle(delta);
		}
	}
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
        if (@event is InputEventScreenTouch TouchEvent && @event.IsPressed())
        {
            HandleInput(TouchEvent.Position, TouchEvent.Index);
        }
        else if (@event is InputEventScreenDrag DragEvent)
		{
			HandleInput(DragEvent.Position, DragEvent.Index);
		}
		if (@event is InputEventScreenTouch Event && !@event.IsPressed() && Event.Index == OngoingDrag)
		{
			OngoingDrag = -1;
		}
	}
	private void HandleInput(Vector2 EventPosition, int index)
	{
		Sprite2D parent = GetParent() as Sprite2D;
		float distance_from_center = (EventPosition - parent.GlobalPosition).Length();
		if (distance_from_center <= boundary * GlobalScale.X || index == OngoingDrag)
		{
			GlobalPosition = EventPosition - radius * GlobalScale;
			if (GetButtonPosition().Length() > boundary)
			{
				Position = GetButtonPosition().Normalized() * boundary - radius;
			}
			OngoingDrag = index;
		}

	}
	private Vector2 GetButtonPosition() => Position + radius;
    public Vector2 GetValue()
	{
		if (GetButtonPosition().Length() > threshold)
		{
			return GetButtonPosition().Normalized();
		}
		return new Vector2(0, 0);
	}
}
