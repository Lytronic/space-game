using Godot;
using System;

public partial class GameMenu : HBoxContainer
{
	private Node _soundManager;
	public override void _Ready()
	{
		_soundManager = GetNode("/root/SoundManager");
		GetNode<TextureButton>("VBoxContainerRight/TextureRect/SettingsButton").Pressed += () => {
			_soundManager.Call("PlaySound", 0, 0);

			SettingsScreen settings = (SettingsScreen)ResourceLoader.Load<PackedScene>("res://scenes/ui/SettingsScreen.tscn").Instantiate();
			settings.Close += () => Visible = true; // restore visibility when settings close
			GetNode<Node>("..").AddChild(settings);
			Visible = false; // make this invisible
		};
		GetNode<TextureButton>("VBoxContainerRight/TextureRect/HighScoresButton").Pressed += () => {
			_soundManager.Call("PlaySound", 0, 0);

			HighScoresScreen settings = (HighScoresScreen)ResourceLoader.Load<PackedScene>("res://scenes/ui/HighScoresScreen.tscn").Instantiate();
			settings.Close += () => Visible = true;
			GetNode<Node>("..").AddChild(settings);
			Visible = false;
		};
		GetNode<TextureButton>("VBoxContainerRight/TextureRect/QuitButton").Pressed += () => {
			_soundManager.Call("PlaySound", 0, 0);
			GetTree().Quit();
		};
		GetNode<TextureButton>("VBoxContainerLeft/BackButton").Pressed += () => {
			_soundManager.Call("PlaySound", 0, 0);
			GetTree().ChangeSceneToFile("res://scenes/ui/TitleScreen.tscn");
		};
		GetNode<TextureButton>("VBoxContainerRight/TextureRect/LoadGame").Pressed += () => {
			_soundManager.Call("PlaySound", 0, 0);
			GetTree().ChangeSceneToFile("res://scenes/ui/LoadMenu.tscn");
		};
		GetNode<TextureButton>("VBoxContainerRight/TextureRect/NewGame").Pressed += () => {
			_soundManager.Call("PlaySound", 0, 0);
			_soundManager.Call("Fight");
			GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
		};
	}


	public override void _Process(double delta)
	{

	}
}
