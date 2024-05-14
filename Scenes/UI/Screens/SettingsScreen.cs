using System.Collections.Generic;

public partial class SettingsScreen : Control
{
	[Export] private VolumeSlider master;
	[Export] private VolumeSlider music;
	[Export] private VolumeSlider effects;
	[Export] private OptionButton resolutionPicker;
	[Export] private CheckBox fullscreen;
	[Export] private CheckBox vsync;

	private string _settingsPath = "user://settings.cfg";
	private Dictionary<string, Vector2> resolutions = new()
    {
		{ "800x600", new Vector2(800, 600) },
		{ "1024x768", new Vector2(1024, 768) },
		{ "1280x720", new Vector2(1280, 720) },
		{ "1366x768", new Vector2(1366, 768) },
		{ "1600x900", new Vector2(1600, 900) },
		{ "1920x1080", new Vector2(1920, 1080) },
		{ "2560x1440", new Vector2(2560, 1440) },
		{ "3840x2160", new Vector2(3840, 2160) }
	};

	public override void _Ready()
	{
		AddResolutions();
		LoadData();
		SetSettings(resolutionPicker.Text);
	}

	public void CloseButtonPressed()
	{
		this.Visible = false;

		SaveData();
		SetSettings(resolutionPicker.Text);
	}

	private void AddResolutions()
	{
		var currentResolution = GetViewport().GetVisibleRect().Size;

		var index = 0;
		foreach (var resolution in resolutions)
		{
			resolutionPicker.AddItem(resolution.Key, index);

			if (resolution.Value == currentResolution) 
			{
				resolutionPicker.Select(index);
			}

			index++;
		}
	}

	private void SetSettings(string res)
	{
		var resolutionVector = resolutions[res];
		DisplayServer.WindowSetSize((Vector2I)resolutionVector);

		var index = 0;
		foreach (var resolution in resolutions)
		{
			if (res == resolution.Key) 
			{
				resolutionPicker.Select(index);
			}

			index++;
		}

		DisplayServer.WindowSetVsyncMode(vsync.ButtonPressed ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);

		DisplayServer.WindowSetMode(fullscreen.ButtonPressed ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
	}

	public void SaveData()
	{
		var config = new ConfigFile();

		config.SetValue("Audio", "MasterVolume", master.Value);
		config.SetValue("Audio", "MusicVolume", music.Value);
		config.SetValue("Audio", "EffectsVolume", effects.Value);
		config.SetValue("Video", "Resolution", resolutionPicker.Text);
		config.SetValue("Video", "Fullscreen", fullscreen.ButtonPressed);
		config.SetValue("Video", "VSync", vsync.ButtonPressed);

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
		resolutionPicker.Text = (string)config.GetValue("Video", "Resolution");
		fullscreen.ButtonPressed = (bool)config.GetValue("Video", "Fullscreen");
		vsync.ButtonPressed = (bool)config.GetValue("Video", "VSync");
	}
}

public class SettingsData
{

}
