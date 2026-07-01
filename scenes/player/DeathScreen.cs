using Godot;
using Microgravity.util;

public partial class DeathScreen : Control
{
	public override void _Ready()
	{
		GetNode<Label>("VBoxContainer/ScoreLabel").Text = $"Your Score: {PlayerVariables.Instance.Score}";

		GetNode<Button>("QuitButton").Pressed += () => GetTree().ChangeSceneToFile("res://scenes/ui/TitleScreen.tscn");

		Button saveButton = GetNode<Button>("VBoxContainer/HBoxContainer/SaveButton");
		saveButton.Pressed += () =>
		{
			string name = GetNode<LineEdit>("VBoxContainer/HBoxContainer/NameLine").Text.Trim();
			if (DB.AddHighScore(name, PlayerVariables.Instance.Score))
			{
				saveButton.Disabled = true;
				saveButton.Text = "Saved";
			}
		};
	}
	
	private void _on_retry_button_pressed()
	{
		GetTree().ReloadCurrentScene();
	}
}
