using Godot;
using System;

public partial class TitleScreen : VBoxContainer
{
	public override void _Ready()
	{
		GetNode<Button>("./HBoxContainer/SettingsButton").Pressed += () => {
			SettingsScreen settings = (SettingsScreen)ResourceLoader.Load<PackedScene>("res://scenes/ui/SettingsScreen.tscn").Instantiate();
			settings.Close += () => Visible = true; // restore visibility when settings close
			GetNode<Node>("..").AddChild(settings);
			Visible = false; // make this invisible
		};
		
		GetNode<Button>("./HBoxContainer/QuitButton").Pressed += () => GetTree().Quit();
	}
	
	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (@event.IsPressed())
		{
			GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
		}
	}
}
