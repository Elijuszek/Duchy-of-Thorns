namespace DuchyOfThorns;

public partial class Arrow : Projectile
{
    public override void _Ready()
    {
        base._Ready();
    }
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
            fireplace.SetOnFire(team, GlobalPosition, direction);
            RemoveFromScene();
        }
        else
        {
            RemoveFromScene();
        }
    }
}
