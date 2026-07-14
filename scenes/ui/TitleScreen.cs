using Godot;
using System;

public partial class TitleScreen : VBoxContainer
{
	private SoundManager _soundManager;

	public override async void _Ready()
	{
		_soundManager = GetNode<SoundManager>("/root/SoundManager");

		GetNode<TextureButton>("./HBoxContainer/SettingsButton").Pressed += () => {
			_soundManager.PlaySound(0,0);

			SettingsScreen settings = (SettingsScreen)ResourceLoader.Load<PackedScene>("res://scenes/ui/SettingsScreen.tscn").Instantiate();
			settings.Close += () => Visible = true; // restore visibility when settings close
			GetNode<Node>("..").AddChild(settings);
			Visible = false; // make this invisible
		};

		GetNode<TextureButton>("./HBoxContainer/HighScoresButton").Pressed += () => {
			_soundManager.PlaySound(0,0);

			HighScoresScreen settings = (HighScoresScreen)ResourceLoader.Load<PackedScene>("res://scenes/ui/HighScoresScreen.tscn").Instantiate();
			settings.Close += () => Visible = true;
			GetNode<Node>("..").AddChild(settings);
			Visible = false;
		};
		
		GetNode<TextureButton>("./HBoxContainer/QuitButton").Pressed += () => {
			_soundManager.PlaySound(0,0);
			GetTree().Quit();
		};
		_soundManager.PlayIntro();
		_soundManager.EndFlightNoise();
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsPressed()
			&& (@event is InputEventKey || @event is InputEventMouseButton)
			&& Visible)
		{
			_soundManager.Menu();
			GetTree().ChangeSceneToFile("res://scenes/ui/GameMenu.tscn");
		}
	}
}
