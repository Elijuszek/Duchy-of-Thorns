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
		animationPlayer.Play("Idle");
		delivered = false;
	}
	public override void Attack()
	{
		animationPlayer.Play("Attack");
		ChangePitch();
		attackSound.Play();
	}
	public override void Deliver()
	{
		animationPlayer.Play("Idle");
		deliverSound.Stream = punshSounds[rand.Next(0, 4)];
	}
	public override void Walking() => animationPlayer.Play("Idle");
	public void ChangePitch() => attackSound.PitchScale = Globals.GetRandomFloat(0.8f, 1);
}
