using DuchyofThorns.Scenes.Globals;

namespace DuchyOfThorns;

/// <summary>
/// Class for the base which can be captured by the player, ally or enemy
/// </summary>
public partial class CapturableBase : Area2D
{
    [Signal] public delegate void BaseCapturedEventHandler(Team newTeam);
    [Export] Color neutralColor = new Color(1, 1, 1);
    [Export] Color playerColor = new Color(0.431373f, 0.043137f, 0.043137f);
    [Export] Color enemyColor = new Color(0.133333f, 0.345098f, 0.796078f);
    [Export] public Team Team { get; set; } = Team.NEUTRAL;

    private CollisionShape2D collisionShape;
    private Vector2 extents;
    private Sprite2D sprite;
    private Timer captureTimer;
    private ProgressBar captureProgressBar;
    private ProgressBar progressNumbers;
    private Tween progressTween;
    private int playerCount = 0;
    private int enemyCount = 0;
    private Team teamToCapture = Team.NEUTRAL;
    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        extents = (collisionShape.Shape as RectangleShape2D).Size; // was Extents
        sprite = GetNode<Sprite2D>("Sprite2D");
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
        Team majorityTeam = GetMajority();
        if (majorityTeam == Team.NEUTRAL)
        {
            teamToCapture = majorityTeam;
            captureTimer.Stop();
            StopCaptureBar();
        }
        else if (majorityTeam == Team)
        {
            teamToCapture = Team.NEUTRAL;
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
    private Team GetMajority()
    {
        if (enemyCount == playerCount)
        {
            return Team.NEUTRAL;
        }
        else if (enemyCount > playerCount)
        {
            return Team.ENEMY;
        }
        else
        {
            return Team.PLAYER;
        }
    }
    public void SetTeam(Team newTeam)
    {
        Team = newTeam;
        EmitSignal(nameof(BaseCaptured), (int)newTeam);
        progressNumbers.Visible = false;
        captureProgressBar.Visible = false;
        switch (newTeam)
        {
            case Team.NEUTRAL:
                sprite.Modulate = neutralColor;
                return;
            case Team.PLAYER:
                sprite.Modulate = playerColor;
                return;
            case Team.ENEMY:
                sprite.Modulate = enemyColor;
                return;
        }
    }
    private void StartCaptureBar()
    {
        captureProgressBar.Value = 0;
        StyleBoxFlat barStyle = (StyleBoxFlat)captureProgressBar.Get("custom_styles/fg");
        if (teamToCapture == Team.PLAYER)
        {
            captureProgressBar.Modulate = playerColor;
        }
        else if (teamToCapture == Team.ENEMY)
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
            Team bodyTeam = actor.GetTeam();
            if (bodyTeam == Team.ENEMY)
            {
                enemyCount++;
            }
            else if (bodyTeam == Team.PLAYER)
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
            Team bodyTeam = actor.GetTeam();
            if (bodyTeam == Team.ENEMY)
            {
                enemyCount--;
            }
            else if (bodyTeam == Team.PLAYER)
            {
                playerCount--;
            }
            CanBeCaptured();
        }
    }
    private void CaptureTimerTimeout() => SetTeam(teamToCapture);
}
