namespace DuchyOfThorns;

/// <summary>
/// Basic arrow class. Intermediate for all other arrow types
/// </summary>
public partial class Arrow : Projectile
{
    protected virtual void ArrowBodyEntered(Node body)
    {
        if (body is Actor actor)
        {
            if (actor.GetTeam() != team)
            {
                actor.HandleHit(Damage, GlobalPosition);
                RemoveFromScene();
            }
        }
        else if (body is Fireplace fireplace)
        {
            fireplace.SetOnFire(Damage * 2, team, GlobalPosition, direction);
            RemoveFromScene();
        }
        else
        {
            RemoveFromScene();
        }
    }
}
