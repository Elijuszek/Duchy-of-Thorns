using Godot;
using System;

namespace Egzaminas6;

public partial class Player : CharacterBody3D
{
    [Export] public float Speed { get; set; } = 10f;
    [Export] public float JumpVelocity { get; set; } = 4f;
    private float mouseSensitivity = 0.5f;
    public float gravity = 6.8f;

    [Export] private SpringArm3D pivot;

    private Vector3 direction = Vector3.Zero;

    public override void _Ready()
    {
        base._Ready();
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (!IsOnFloor())
        {
            direction.Y -= gravity * (float)delta;
        }

        Velocity = direction * Speed;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion mouseMotion)
        {
            Vector3 rotDeg = RotationDegrees;
            rotDeg.Y -= mouseMotion.Relative.X * mouseSensitivity;
            RotationDegrees = rotDeg;

            rotDeg = pivot.RotationDegrees;
            rotDeg.X -= mouseMotion.Relative.Y * mouseSensitivity;
            rotDeg.X = Mathf.Clamp(rotDeg.X, -90f, 90f);
            pivot.RotationDegrees = rotDeg;
        }

        Vector2 input = Input.GetVector("LEFT", "RIGHT", "UP", "DOWN");
        if (Input.IsActionJustPressed("JUMP"))
        {
            direction.Y = JumpVelocity;
        }

        direction = new Vector3(input.X, direction.Y, input.Y).Rotated(Vector3.Up, pivot.GlobalRotation.Y);
    }
}
