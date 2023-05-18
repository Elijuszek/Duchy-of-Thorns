using Godot;

public partial class FireArrow : Arrow
{
    GpuParticles2D fire;
    GpuParticles2D embers;
    public override void _Ready()
    {
        base._Ready();
        fire = GetNode<GpuParticles2D>("Fire");
        embers = GetNode<GpuParticles2D>("Embers");
    }
    public void StartEmiting()
    {
        fire.Emitting = true;
        embers.Emitting = true;

    }
    public void StopEmiting()
    {
        fire.Emitting = false;
        embers.Emitting = false;
    }
    protected override void ArrowBodyEntered(Node body)
    {
        if (body is Actor actor)
        {
            if (actor.GetTeam() != team)
            {
                actor.HandleHit(Damage, GlobalPosition);
                RemoveFromScene();
            }
        }
        else if (body is Fireplace)
        {
            return;
        }
        else
        {
            RemoveFromScene();
        }
    }
}
