namespace DuchyOfThorns;

/// <summary>
/// Class for fists functionality, default player weapon
/// </summary>
public partial class Fists : Melee
{
    [Export] private AnimationPlayer animationPlayer;
    private Random rand;
    private AudioStreamWav[] punshSounds =
        {
            ResourceLoader.Load<AudioStreamWav>("res://Sounds/Melee/Fists/Punch-1.wav"),
            ResourceLoader.Load<AudioStreamWav>("res://Sounds/Melee/Fists/Punch-2.wav"),
            ResourceLoader.Load<AudioStreamWav>("res://Sounds/Melee/Fists/Punch-3.wav"),
            ResourceLoader.Load<AudioStreamWav>("res://Sounds/Melee/Fists/Punch-4.wav"),
            ResourceLoader.Load<AudioStreamWav>("res://Sounds/Melee/Fists/Punch-5.wav"),
        };
    public override void _Ready()
    {
        base._Ready();
        rand = new Random();
    }
    public override void Idle()
    {
        base.Idle();
        animationPlayer.Play("Idle");
    }
    public override bool CanAttack()
    {
        return base.CanAttack() && animationPlayer.CurrentAnimation != "Attack";
    }
    public override void Attack()
    {
        base.Attack();
        animationPlayer.Play("Attack");
        ChangePitch();
        attackSound.Play();
    }
    public override void Deliver()
    {
        base.Deliver();
        animationPlayer.Play("Idle");
    }
    public override void Area2DBodyEntered(Node body)
    {
        if (body is Actor actor && actor.GetTeam() != team)
        {
            actor.HandleHit(damage, GlobalPosition);
            deliverSound.Stream = punshSounds[rand.Next(0, 4)];
            deliverSound.Play();
        }
    }
    public override void Walking() => animationPlayer.Play("Idle");
    public void ChangePitch() => attackSound.PitchScale = Globals.GetRandomFloat(0.8f, 1);
}
