namespace DuchyOfThorns;

/// <summary>
/// Final arrow class
/// </summary>
public partial class FireSpell : Spell
{
    [Export] private GpuParticles2D fire;
    [Export] private GpuParticles2D embers;
    [Export] private Timer timer;

    public override void _PhysicsProcess(double delta)
    {
        if (direction != Vector2.Zero)
        {
            Vector2 velocity = direction * Speed;
            GlobalPosition += velocity;
            moveAmount = Convert.ToSingle(delta) * Speed;
            traveledDistance += moveAmount;
            if (traveledDistance > Range)
            {
                RemoveFromScene();
            }
        }
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
    protected override void SpellBodyEntered(Node body)
    {
        switch (body)
        {
            case Actor actor:
                if (actor.GetTeam() != team)
                {
                    actor.HandleHit(Damage, GlobalPosition);
                    RemoveFromScene();
                }
                break;

            case Fireplace fireplace:
                break;

            default:
                RemoveFromScene();
                break;
        }
    }
    public override void AddToScene()
    {
        base.AddToScene();
        Speed = 3f;
        StartEmiting();
    }
    public override void RemoveFromScene()
    {
        Speed = 0f;
        StopEmiting();
        base.RemoveFromScene();
    }
}
