using Godot;

public partial class Background : Node2D
{
	[Export] public NodePath playerPath;

	private CharacterBody2D _player;
	private Camera2D _camera;

	public override void _Ready()
	{
		_player = GetNode<CharacterBody2D>(playerPath);
		_camera = _player.GetNode<Camera2D>("Camera2D");
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (_camera == null)
			return;

		Vector2 viewportSize = GetViewportRect().Size * _camera.Zoom;
		Vector2 cameraCenter = _camera.GlobalPosition;

		float left = cameraCenter.X - viewportSize.X * 0.5f;
		float top = cameraCenter.Y - viewportSize.Y * 0.5f;

		DrawRect(new Rect2(new Vector2(left, top), viewportSize), Colors.Black, true);
	}
}
