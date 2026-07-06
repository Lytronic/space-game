using Godot;
using Microgravity.util;

public partial class DeathScreen : Control
{
	private Node _soundManager;

	public override void _Ready()
	{
		_soundManager = GetNode("/root/SoundManager");
		GetNode<Label>("VBoxContainer/ScoreLabel").Text = $"Your Score: {PlayerVariables.Instance.Score}";

		GetNode<Button>("QuitButton").Pressed += () => {
			_soundManager.Call("PlaySound", 0, 0);
			PlayerVariables.Instance.ResetRun();
			GetTree().ChangeSceneToFile("res://scenes/ui/TitleScreen.tscn");
		};	

		Button saveButton = GetNode<Button>("VBoxContainer/HBoxContainer/SaveButton");
		saveButton.Pressed += () =>
		{
			_soundManager.Call("PlaySound", 0, 0);
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
		_soundManager.Call("PlaySound", 0, 0);
		PlayerVariables.Instance.ResetRun();
		GetTree().ReloadCurrentScene();
	}
}
