namespace DuchyOfThorns;

/// <summary>
/// Class for aiming functionality
/// </summary>
public partial class Aiming : Sprite2D
{
    private Tween tween;
    public override void _Ready()
    {
        Visible = false;
    }
    public void StartAiming(float startScale, float endScale, float duration, float delay)
    {
        Scale = new Vector2(Scale.X, startScale);
        Visible = true;
        tween = CreateTween();
        tween.TweenInterval(delay);
        tween.SetTrans(Tween.TransitionType.Linear);
        tween.SetEase(Tween.EaseType.In);
        tween.TweenProperty(this, "scale:y", endScale, duration);
    }
    public float GetDirection()
    {
        tween.Stop();
        Visible = false;
        Random rand = new Random();
        double max = (Texture.GetHeight() / 2 * Scale.Y);
        double min = -max;
        double range = max - min;
        double sample = rand.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }
    public void Cancel()
    {
        if (tween != null)
        {
            tween.Stop();
        }
        Visible = false;
    }
}
