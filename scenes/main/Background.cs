using Godot;

public partial class Background : Node2D
{
    [Export] public float CellSize = 64.0f;
    [Export] public Color LineColor = new Color(0.25f, 0.25f, 0.30f, 1.0f);
    [Export] public float LineWidth = 1.0f;
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
        if (_camera == null || CellSize <= 0.0f)
            return;

        Vector2 viewportSize = GetViewportRect().Size * _camera.Zoom;
        Vector2 cameraCenter = _camera.GlobalPosition;

        float left = cameraCenter.X - viewportSize.X * 0.5f;
        float right = cameraCenter.X + viewportSize.X * 0.5f;
        float top = cameraCenter.Y - viewportSize.Y * 0.5f;
        float bottom = cameraCenter.Y + viewportSize.Y * 0.5f;

        int startX = Mathf.FloorToInt(left / CellSize) - 1;
        int endX = Mathf.CeilToInt(right / CellSize) + 1;
        int startY = Mathf.FloorToInt(top / CellSize) - 1;
        int endY = Mathf.CeilToInt(bottom / CellSize) + 1;

        for (int x = startX; x <= endX; x++)
        {
            float worldX = x * CellSize;
            DrawLine(
                ToLocal(new Vector2(worldX, top)),
                ToLocal(new Vector2(worldX, bottom)),
                LineColor,
                LineWidth
            );
        }

        for (int y = startY; y <= endY; y++)
        {
            float worldY = y * CellSize;
            DrawLine(
                ToLocal(new Vector2(left, worldY)),
                ToLocal(new Vector2(right, worldY)),
                LineColor,
                LineWidth
            );
        }
    }
}