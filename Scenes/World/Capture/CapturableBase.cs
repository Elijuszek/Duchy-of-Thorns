namespace DuchyOfThorns;

/// <summary>
/// Class for the base which can be captured by the player, ally or enemy
/// </summary>
public partial class CapturableBase : Area2D
{
    [Signal] public delegate void BaseCapturedEventHandler(int newTeam);
    [Export] Color neutralColor = new Color(1, 1, 1);
    [Export] Color playerColor = new Color(0.431373f, 0.043137f, 0.043137f);
    [Export] Color enemyColor = new Color(0.133333f, 0.345098f, 0.796078f);
    private CollisionShape2D collisionShape;
    private Vector2 extents;
    private Sprite2D sprite;
    private Timer captureTimer;
    private ProgressBar captureProgressBar;
    private ProgressBar progressNumbers;
    private Tween progressTween;
    private int playerCount = 0;
    private int enemyCount = 0;
    private int teamToCapture = (int)Team.TeamName.NEUTRAL;
    private const int teamNeutral = (int)Team.TeamName.NEUTRAL;
    private const int teamPlayer = (int)Team.TeamName.PLAYER;
    private const int teamEnemy = (int)Team.TeamName.ENEMY;
    public Team Team { get; set; }
    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        extents = (collisionShape.Shape as RectangleShape2D).Size; // was Extents
        sprite = GetNode<Sprite2D>("Sprite2D");
        Team = GetNode<Team>("Team");
        captureTimer = GetNode<Timer>("CaptureTimer");
        captureProgressBar = GetNode<ProgressBar>("ProgressBar");
        progressNumbers = GetNode<ProgressBar>("ProgressNumbers");
    }
    public Vector2 GetRandomPositionWithinRadius()
    {
        Vector2 topLeft = collisionShape.GlobalPosition - (extents / 2);
        float x = Globals.GetRandomFloat(topLeft.X, topLeft.X + extents.X);
        float y = Globals.GetRandomFloat(topLeft.Y, topLeft.Y + extents.Y);
        return new Vector2(x, y);
    }
    public void CanBeCaptured()
    {
        int majorityTeam = GetMajority();
        if (majorityTeam == teamNeutral)
        {
            teamToCapture = teamNeutral;
            captureTimer.Stop();
            StopCaptureBar();
        }
        else if (majorityTeam == (int)Team.team)
        {
            teamToCapture = teamNeutral;
            captureTimer.Stop();
            StopCaptureBar();
        }
        else if (captureTimer.IsStopped())
        {
            teamToCapture = majorityTeam;
            captureTimer.Start();
            StartCaptureBar();
        }

    }
    private int GetMajority()
    {
        if (enemyCount == playerCount)
        {
            return (int)Team.TeamName.NEUTRAL;
        }
        else if (enemyCount > playerCount)
        {
            return (int)Team.TeamName.ENEMY;
        }
        else
        {
            return (int)Team.TeamName.PLAYER;
        }
    }
    public void SetTeam(int newTeam)
    {
        Team.team = (Team.TeamName)newTeam;
        EmitSignal(nameof(BaseCaptured), newTeam);
        progressNumbers.Visible = false;
        captureProgressBar.Visible = false;
        switch (newTeam)
        {
            case teamNeutral:
                sprite.Modulate = neutralColor;
                return;
            case teamPlayer:
                sprite.Modulate = playerColor;
                return;
            case teamEnemy:
                sprite.Modulate = enemyColor;
                return;
        }
    }
    private void StartCaptureBar()
    {
        captureProgressBar.Value = 0;
        StyleBoxFlat barStyle = (StyleBoxFlat)captureProgressBar.Get("custom_styles/fg");
        if (teamToCapture == teamPlayer)
        {
            captureProgressBar.Modulate = playerColor;
        }
        else if (teamToCapture == teamEnemy)
        {
            captureProgressBar.Modulate = enemyColor;
        }
        progressTween = CreateTween();
        progressTween.TweenProperty(captureProgressBar, "value", 100, captureTimer.WaitTime);
        progressTween.TweenProperty(progressNumbers, "value", 100, captureTimer.WaitTime);
        progressTween.SetTrans(Tween.TransitionType.Linear);
        progressTween.SetEase(Tween.EaseType.In);
        progressNumbers.Visible = true;
        captureProgressBar.Visible = true;
    }
    private void StopCaptureBar()
    {
        StyleBoxFlat barStyle = (StyleBoxFlat)captureProgressBar.Get("custom_styles/fg");
        progressTween = CreateTween();
        progressTween.TweenProperty(captureProgressBar, "value", 0, captureTimer.WaitTime);
        progressTween.TweenProperty(progressNumbers, "value", 0, captureTimer.WaitTime);
        progressTween.SetTrans(Tween.TransitionType.Linear);
        progressTween.SetEase(Tween.EaseType.In);
    }
    private void CapturableBaseBodyEntered(Node body)
    {
        if (body is Actor actor)
        {
            int bodyTeam = actor.GetTeam();
            if (bodyTeam == teamEnemy)
            {
                enemyCount++;
            }
            else if (bodyTeam == teamPlayer)
            {
                playerCount++;
            }
            CanBeCaptured();
        }
    }
    private void CapturableBaseBodyExited(Node body)
    {
        if (body is Actor actor)
        {
            int bodyTeam = actor.GetTeam();
            if (bodyTeam == teamEnemy)
            {
                enemyCount--;
            }
            else if (bodyTeam == teamPlayer)
            {
                playerCount--;
            }
            CanBeCaptured();
        }
    }
    private void CaptureTimerTimeout()
    {
        SetTeam(teamToCapture);
    }
}
