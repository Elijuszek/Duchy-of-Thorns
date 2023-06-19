global using Godot;
global using System;
global using System.Linq;
global using DuchyofThorns.Scenes.Globals;

namespace DuchyOfThorns;

/// <summary>
/// Class for defining global variables and functionality
/// </summary>
public partial class Globals : Node
{
	[Signal]
	public delegate void ArrowFiredEventHandler(Arrow arrow, Team team, Marker2D position, Vector2 direction);
	[Signal]
	public delegate void CoinsDropedEventHandler(int coins, Marker2D position);
	private const string SaveDir = "user://saves/";
	private string savePath = SaveDir + "save.dat";
	private PackedScene transitionScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Transitions/PixelationTransition.tscn");
	private PixelationTransition transition;

	public LoadingForm loadingForm { get; set; } = LoadingForm.New;
	public Godot.Collections.Dictionary<string, Variant> Player { get; set; }
	public bool SaveGame()
	{
		if (!DirAccess.DirExistsAbsolute(SaveDir))
		{
			DirAccess.MakeDirRecursiveAbsolute(SaveDir);
		}
		var saveFile = Godot.FileAccess.OpenEncryptedWithPass(savePath, Godot.FileAccess.ModeFlags.Write, "Big8008135!");
		if (saveFile.GetError() != Error.Ok)
		{
			GD.PushError("Failed to save data!");
			return false;
		}
		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
		{
			if (!saveNode.HasMethod("Save") || saveNode.Name.IsEmpty)
			{
				continue;
			}
			var data = saveNode.Call("Save");
			GD.Print(data);
			saveFile.StoreLine(Json.Stringify(data));
		}
		return true;
	}

	public async void LoadGame()
	{
		if (!Godot.FileAccess.FileExists(savePath))
		{
			GetTree().ChangeSceneToFile("res://Scenes/UI/TitleScreen.tscn");
			return;
		}

		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
		{
			saveNode.QueueFree();
		}
		GetTree().CurrentScene.QueueFree();
		if (saveNodes.Count > 0)
		{
			await ToSignal(GetTree().CreateTimer(0.00001f), "timeout");
		}
		var saveFile = Godot.FileAccess.OpenEncryptedWithPass(savePath, Godot.FileAccess.ModeFlags.Read, "Big8008135!");
		if (saveFile.GetError() != Error.Ok)
		{
			GD.PushError("Failed to load data!");
		}
		while (saveFile.GetPosition() < saveFile.GetLength())
		{
			var data = (Godot.Collections.Dictionary<string, Variant>)Json.ParseString(saveFile.GetLine());
			var newObjectScene = (PackedScene)ResourceLoader.Load(data["Filename"].ToString());
			var newObject = (Node)newObjectScene.Instantiate();
			newObject.Set("position", new Vector2((float)data["PosX"], (float)data["PosY"]));
			GetNode(data["Parent"].ToString()).AddChild(newObject);
			newObject.Call("Load", data);
		}
		GetTree().CurrentScene.Call("LoadSavedPlayer"); // HOTFIX
	}
	public void ChangeScenes(Player player, string scene, float speed)
	{
		this.Player = player.Save();
		loadingForm = LoadingForm.Load;
		transition = transitionScene.Instantiate() as PixelationTransition;
		GetNode("/root").AddChild(transition);
		transition.PlayInOut(scene, speed);
	}
	public void StopChangingScenes() => transition.Stop();
    public static float GetRandomFloat(float min, float max)
	{
		Random rand = new Random();
		float range = max - min;
		double sample = rand.NextDouble();
		double scaled = (sample * range) + min;
		return (float)scaled;
	}
}
