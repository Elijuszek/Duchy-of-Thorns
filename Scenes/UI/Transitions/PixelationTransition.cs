namespace DuchyOfThorns;
public partial class PixelationTransition : CanvasLayer
{
    private AnimationPlayer transition;
    private ColorRect textureRect;
    private string nextScenePath = "";
    private Timer timer;
    private float currentTime;
    public override void _Ready()
    {
        base._Ready();
        transition = GetNode<AnimationPlayer>("AnimationPlayer");
        textureRect = GetNode<ColorRect>("TextureRect");
        timer = GetNode<Timer>("Timer");
    }
    public void PlayInOut(string scenePath, float speed)
    {
        nextScenePath = scenePath;
        transition.SpeedScale = speed;
        timer.WaitTime /= speed;
        timer.Start();
        transition.Play("PixelateInOut");
        currentTime = GetTree().CurrentScene.GetNode<DayNightCycle>("DayNightCycle").Time;
    }
    public void Stop()
    {
        if (!timer.IsStopped())
        {
            timer.Stop();
            transition.Play("PixelateOut");
        }
    }
    private void TimerTimeout()
    {
        GetTree().ChangeSceneToFile(nextScenePath);
    }

}
