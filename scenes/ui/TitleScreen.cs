using Godot;
using System;

public partial class TitleScreen : VBoxContainer
{
	public override void _Ready()
	{
		GetNode<Button>("./HBoxContainer/SettingsButton").Pressed += () => GetTree().ChangeSceneToFile("res://scenes/ui/SettingsScreen.tscn");
		GetNode<Button>("./HBoxContainer/QuitButton").Pressed += () => GetTree().Quit();
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey)
		{
			GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
		}
	}
}
