namespace DuchyOfThorns;

/// <summary>
/// Class which determines which team an actor belongs to
/// </summary>
public partial class Team : Node2D
{
    public enum TeamName
    {
        NEUTRAL,
        PLAYER,
        ENEMY
    }
    [Export] public TeamName team { get; set; } = TeamName.NEUTRAL;
}
