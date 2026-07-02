using Godot;

public partial class PauseScreen : VBoxContainer
{
	private Control _hud;
	private Environment _env;

	private Node _soundManager;

	public override void _Ready()
	{
		_soundManager = GetNode("/root/SoundManager");
		GetNode<TextureButton>("./ButtonsContainer/Buttons/Resume").Pressed += () => {
			_soundManager.Call("PlaySound", 0);
			Toggle();
		};
		GetNode<TextureButton>("./ButtonsContainer/Buttons/Settings").Pressed += () => {
			_soundManager.Call("PlaySound", 0);
			SettingsScreen settings = (SettingsScreen)ResourceLoader.Load<PackedScene>("res://scenes/ui/SettingsScreen.tscn").Instantiate();
			settings.Close += () => Visible = true; // restore visibility when settings close
			GetNode<CanvasLayer>("..").AddChild(settings);
			Visible = false; // make this invisible
		};

		GetNode<TextureButton>("./ButtonsContainer/Buttons/Quit").Pressed += () => {
			_soundManager.Call("PlaySound", 0);
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
		if (@event.IsActionPressed("ui_close_dialog"))
		{
			Toggle();
		}
	}

	private void Toggle()
	{
		
		if (Input.MouseMode == Input.MouseModeEnum.Visible) {
			Input.SetMouseMode(Input.MouseModeEnum.Hidden);
		}
		else if (Input.MouseMode == Input.MouseModeEnum.Hidden) {
			Input.SetMouseMode(Input.MouseModeEnum.Visible);
		}
		GetTree().Paused = !GetTree().Paused;
		_hud.Visible = !_hud.Visible;
		Visible = !Visible;
		_env.GlowEnabled = !_env.GlowEnabled;
	}
}
