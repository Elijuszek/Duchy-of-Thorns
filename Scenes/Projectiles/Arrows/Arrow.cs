namespace DuchyOfThorns;

/// <summary>
/// Basic arrow class. Intermediate for all other arrow types
/// </summary>
public partial class Arrow : Projectile
{
    protected virtual void ArrowBodyEntered(Node body)
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
                fireplace.SetOnFire(Damage * 2, team, GlobalPosition, direction);
                RemoveFromScene();
                break;
            default:
                RemoveFromScene();
                break;
        }
    }
}
