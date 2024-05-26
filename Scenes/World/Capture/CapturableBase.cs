using Godot.NativeInterop;

namespace DuchyOfThorns;

/// <summary>
/// 
/// TODO: Rework entire capturableBase, introduce new textures and animations
/// 
/// 
/// Class for the base which can be captured by the player, ally or enemy
/// </summary>
public partial class CapturableBase : StaticBody2D
{
    [Signal] public delegate void BaseCapturedEventHandler(Team newTeam);

    [Export] Color neutralColor =  new Color(1, 1, 1);
    [Export] Color playerColor = new Color(0.431373f, 0.043137f, 0.043137f);
    [Export] Color enemyColor = new Color (0.133333f, 0.345098f, 0.796078f);
    [Export] public Team Team { get; set; } = Team.NEUTRAL;
    [Export] private ProgressBar captureProgressBar;
    [Export] private CollisionShape2D captureArea;
    [Export] private double captureTime = 60d;
    private StyleBoxFlat barStyle;
    private Vector2 extents;
    private Tween progressTween;
    private int playerCount = 0;
    private int enemyCount = 0;
    private Team capturingTeam = Team.NEUTRAL;
    public override void _Ready()
    {
        extents = captureArea.Shape.GetRect().Size;
        barStyle = (StyleBoxFlat)captureProgressBar.Get("theme_override_styles/fill");
        capturingTeam = Team;
        switch(Team)
        {
            case Team.PLAYER:
                barStyle.BgColor = playerColor;
                captureProgressBar.Value = 100;
                break;
            case Team.ENEMY:
                barStyle.BgColor = enemyColor;
                captureProgressBar.Value = 100;
                break;
            case Team.NEUTRAL:
                barStyle.BgColor = neutralColor;
                captureProgressBar.Value = 0;
                break;
        }
    }
    public void SetTeam()
    {
        switch (Team)
        {
            case Team.PLAYER:
                barStyle.BgColor = playerColor;
                captureProgressBar.Value = 100;
                break;
            case Team.ENEMY:
                barStyle.BgColor = enemyColor;
                captureProgressBar.Value = 100;
                break;
            case Team.NEUTRAL:
                barStyle.BgColor = neutralColor;
                captureProgressBar.Value = 0;
                break;
        }
        Team = capturingTeam;
        EmitSignal(nameof(BaseCaptured), (int)capturingTeam);
    }
    public void SetTeam(Team newTeam)
    {
        Team = newTeam;
        EmitSignal(nameof(BaseCaptured), (int)newTeam);
    }
    public Vector2 GetDestination()
    {
        return Utilities.GetRandomPositionInArea(captureArea);
    }
    public void Capture()
    {
        Team majority = GetMajority();
        if (capturingTeam == majority) return;
        capturingTeam = majority;
        StopCaptureBar();
    }
    private Team GetMajority()
    {
        return (enemyCount == playerCount) ? Team.NEUTRAL :
               (enemyCount > playerCount) ? Team.ENEMY : Team.PLAYER;
    }
    private void StartCaptureBar()
    {
        switch (capturingTeam)
        {
            case Team.NEUTRAL:
                barStyle.BgColor = neutralColor;
                return;
            case Team.PLAYER:
                barStyle.BgColor = playerColor;
                break;
            case Team.ENEMY:
                barStyle.BgColor = enemyColor;
                break;
        }
        progressTween = CreateTween();
        progressTween.TweenProperty(captureProgressBar, "value", 100, captureTime);
        progressTween.SetTrans(Tween.TransitionType.Linear);
        progressTween.SetEase(Tween.EaseType.In);
        progressTween.Connect("finished", new Callable(this, "SetTeam"));
    }
    private void StopCaptureBar()
    {
        if (captureProgressBar.Value == 100 && capturingTeam == Team.NEUTRAL) return;
        progressTween = CreateTween();
        progressTween.TweenProperty(captureProgressBar, "value", 0, captureTime * (captureProgressBar.Value / 100));
        progressTween.SetTrans(Tween.TransitionType.Linear);
        progressTween.SetEase(Tween.EaseType.In);
        progressTween.Connect("finished", new Callable(this, "StartCaptureBar"));
    }
    private void CapturableBaseBodyEntered(Node body)
    {
        GD.Print("Enemy entered");
        if (body is Actor actor)
        {
            Team bodyTeam = actor.GetTeam();
            switch (bodyTeam)
            {
                case Team.ENEMY:

                    enemyCount++;
                    Capture();
                    break;
                case Team.PLAYER:
                    playerCount++;
                    Capture();
                    break;
                default:
                    return;
            }
        }
    }
    private void CapturableBaseBodyExited(Node body)
    {
        if (body is Actor actor)
        {
            Team bodyTeam = actor.GetTeam();
            switch (bodyTeam)
            {
                case Team.ENEMY:
                    enemyCount--;
                    Capture();
                    break;
                case Team.PLAYER:
                    playerCount--;
                    Capture();
                    break;
                default:
                    return;
            }
        }
    }
}
