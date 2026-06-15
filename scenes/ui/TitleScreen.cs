using Godot;
using System;

public partial class TitleScreen : VBoxContainer
{
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey)
		{
			GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
		}
	}
}
