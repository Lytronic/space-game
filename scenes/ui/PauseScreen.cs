using Godot;

public partial class PauseScreen : VBoxContainer
{
	private Control _hud;
	private Environment _env;

	public override void _Ready()
	{
		GetNode<Button>("./ButtonsContainer/Buttons/Resume").Pressed += Toggle;
		GetNode<Button>("./ButtonsContainer/Buttons/Settings").Pressed += () => {
			SettingsScreen settings = (SettingsScreen)ResourceLoader.Load<PackedScene>("res://scenes/ui/SettingsScreen.tscn").Instantiate();
			settings.Close += () => Visible = true; // restore visibility when settings close
			GetNode<CanvasLayer>("..").AddChild(settings);
			Visible = false; // make this invisible
		};

		GetNode<Button>("./ButtonsContainer/Buttons/Quit").Pressed += () => {
			GetTree().Paused = false;
			GetTree().ChangeSceneToFile("res://scenes/ui/TitleScreen.tscn");
		};

		_hud = GetNode<Control>("../HUD");
		_env = GetNode<WorldEnvironment>("./WorldEnvironment").Environment;

		_env.BackgroundMode = Environment.BGMode.Canvas;
		_env.GlowEnabled = false;
		_env.GlowNormalized = true;
		_env.GlowIntensity = 1.0f;
		_env.GlowBloom = 1.0f;
		_env.GlowBlendMode = Environment.GlowBlendModeEnum.Replace;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("pause_or_back"))
		{
			Toggle();
		}
	}

	private void Toggle()
	{
		GetTree().Paused = !GetTree().Paused;
		_hud.Visible = !_hud.Visible;
		Visible = !Visible;
		_env.GlowEnabled = !_env.GlowEnabled;
	}
}
