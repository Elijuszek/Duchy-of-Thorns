namespace DuchyOfThorns;

/// <summary>
/// TODO: Background sounds should change according to the time of day
/// Class for day/night cycle functionality
/// </summary>
public partial class DayNightCycle : CanvasModulate
{
    [Export] public float Time { get; set; } = 30;
    [Export] private int dayLenght = 720; // 30s = 1h
    [Export] private bool active = true;
    [Export] private AnimationPlayer animationPlayer;
    [Export] private AudioStreamPlayer nightSounds;

    public override void _Ready()
    {
        if (!active)
        {
            SetProcess(false);
            return;
        }
        //Random rand = new Random();
        //Time = rand.Next(0, dayLenght);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Time += Convert.ToSingle(delta);
        Time = Time % dayLenght;
        float currentFrame = Remap(Time, 0, dayLenght, 0, 24);
        animationPlayer.Play("DayNightCycle");
        animationPlayer.Seek(currentFrame);

        if (currentFrame >= 20 || currentFrame <= 6)
        {
            if (nightSounds.Playing == false)
                nightSounds.Play();
        }
        else
        {
            nightSounds.Stop();
        }

    }

    public float Remap(float value, float istart, float istop, float ostart, float ostop)
    {
        return ostart + (ostop - ostart) * value / (istop - istart);
    }
}
