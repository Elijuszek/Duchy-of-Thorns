namespace DuchyOfThorns;

public partial class VolumeSlider : HSlider
{
	[Export] private string busName;
	private int busIndex;

	public override void _Ready()
	{
		busIndex = AudioServer.GetBusIndex(busName);
		
		Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(busIndex));
	}

    public void OnValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(value));
	}
}
