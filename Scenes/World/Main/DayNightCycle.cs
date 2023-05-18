using Godot;
using System;

public partial class DayNightCycle : CanvasModulate
{
    [Export] public float Time { get; set; } = 30;
    [Export] private int dayLenght = 180; // 30s = 1h
    [Export] private bool active = true;
    private AnimationPlayer animationPlayer;
    public override void _Ready()
    {
        if (!active)
        {
            SetProcess(false);
            return;
        }
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
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
    }
    public float Remap(float value, float istart, float istop, float ostart, float ostop)
    {
        return ostart + (ostop - ostart) * value / (istop - istart);
    }
}
