namespace DuchyOfThorns;
public partial class Blood : GpuParticles2D
{
    private Tween tween;
    public override void _Ready()
    {
        Emitting = true;
        tween = CreateTween();
        //tween.Connect("Finished", new Callable(this, "TweenAllCompleted"));
        tween.TweenProperty(this, "modulate:a", 0, 2.5f);
        tween.TweenCallback(new Callable(this, "TweenAllCompleted"));
    }
    private void TweenAllCompleted()
    {
        QueueFree();
    }
}
