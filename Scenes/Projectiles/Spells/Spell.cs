namespace DuchyOfThorns;

/// <summary>
/// Basic arrow class. Intermediate for all other arrow types
/// </summary>
public partial class Spell : Projectile
{
    protected virtual void SpellBodyEntered(Node body)
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
