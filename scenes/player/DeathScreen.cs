using Godot;
using Microgravity.util;
using System;

public partial class DeathScreen : Control
{
	public override void _Ready()
	{
		GetNode<Label>("VBoxContainer/ScoreLabel").Text = $"Your Score: {PlayerVariables.Instance.Score}";

		GetNode<Button>("QuitButton").Pressed += () => GetTree().ChangeSceneToFile("res://scenes/ui/TitleScreen.tscn");
		
		GetNode<Button>("VBoxContainer/HBoxContainer/SaveButton").Pressed += () =>
		{
			string name = GetNode<LineEdit>("VBoxContainer/HBoxContainer/NameLine").Text;
			if (name.Length > 0)
			{
				DB.AddHighScore(name, PlayerVariables.Instance.Score);
			}
		};
	}
	
	private void _on_retry_button_pressed()
	{
		GetTree().ReloadCurrentScene();
	}
}
