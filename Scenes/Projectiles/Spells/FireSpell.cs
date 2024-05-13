namespace DuchyOfThorns;

/// <summary>
/// Final arrow class
/// </summary>
public partial class FireSpell : Spell
{
    [Export] private GpuParticles2D fire;
    [Export] private GpuParticles2D embers;

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
}
