public partial class SettingsScreen : Control
{
	[Export] private VolumeSlider master;
	[Export] private VolumeSlider music;
	[Export] private VolumeSlider effects;

	private string _settingsPath = "user://TESTHELLOTEST.cfg";

	public override void _Ready()
	{
		LoadData();
	}

	public void CloseButtonPressed()
	{
		this.Visible = false;

		SaveData();
	}

	public void SaveData()
	{
		var config = new ConfigFile();

		config.SetValue("Audio", "MasterVolume", master.Value);
		config.SetValue("Audio", "MusicVolume", music.Value);
		config.SetValue("Audio", "EffectsVolume", effects.Value);

		config.Save(_settingsPath);
	}

	public void LoadData()
	{
		var config = new ConfigFile();

		Error err = config.Load(_settingsPath);
		if (err != Error.Ok) return;

		master.Value = (double)config.GetValue("Audio", "MasterVolume");
		music.Value = (double)config.GetValue("Audio", "MusicVolume");
		effects.Value = (double)config.GetValue("Audio", "EffectsVolume");
	}
}

public class SettingsData
{

}
