global using Godot;
global using System;
global using System.Linq;
namespace DuchyOfThorns;

/// <summary>
/// Class for defining global variables and functionality
/// </summary>
public partial class Globals : Node
{
	private const string SaveDir = "user://saves/";
	private const string savePath = SaveDir + "save.dat";
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
			GetTree().ChangeSceneToFile("res://Scenes/UI/Screens/TitleScreen.tscn");
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
			var newObjectScene = ResourceLoader.Load<PackedScene>(data["Filename"].ToString());
			var newObject = newObjectScene.Instantiate<Node>();
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
}
