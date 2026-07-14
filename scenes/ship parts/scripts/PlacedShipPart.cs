using Godot;

public partial class PlacedShipPart : RigidBody2D
{
    public ShipPart Data { get; private set; }

    private Sprite2D _sprite;
    private CollisionShape2D _collider;

    private bool _isHovered = false;
    private bool _isAttached = false;

    private Vector2 _lastMousePos;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _collider = GetNode<CollisionShape2D>("CollisionShape2D");

        if (Data != null && Data.SpriteTexture != null)
            _sprite.Texture = Data.SpriteTexture;

        InputPickable = true;
        MouseEntered += _on_mouse_entered;
        MouseExited += _on_mouse_exited;
    }

    public void Initialize(ShipPart data)
    {
        Data = data;

        if (_sprite != null && Data.SpriteTexture != null)
            _sprite.Texture = Data.SpriteTexture;
    }

    Vector2 ClampToScreen(Vector2 pos, Vector2 size)
    {
        Vector2 screen = GetViewportRect().Size;

        pos.X = Mathf.Clamp(pos.X, 0, screen.X - size.X);
        pos.Y = Mathf.Clamp(pos.Y, 0, screen.Y - size.Y);

        return pos;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isAttached)
        {
            Vector2 vec = GetGlobalMousePosition() - GlobalPosition;
            LinearVelocity = vec * 5;
        }
    }

    private void _on_body_shape_entered(Node body)
    {
        if (body is PlacedShipPart otherPart && !_isAttached)
        {
            // Get vector away from other part
            Vector2 vec = Position - otherPart.Position;
            Vector2 dir = vec.Normalized();
            float strength = (_collider.Transform.Scale.X * 10 * 2) - vec.Length(); // 10 is std scale; x2 bc rad from both colliders together
            otherPart.LinearVelocity = dir * strength / 2; // x0.5 bc x1 is too much
        }
    }

    private void _on_mouse_entered()
    {
        if (!_isHovered)
        {
            _isHovered = true;
            _sprite.Scale *= 1.125f;
        }
    }

    private void _on_mouse_exited()
    {
        if (_isHovered)
        {
            _isHovered = false;
            _sprite.Scale /= 1.125f;
        }
    }

    private void _on_input_event(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.ButtonIndex == MouseButton.Left &&
            mouseEvent.Pressed)
        {
            Grab();
        }
    }

    private void Grab()
    {
        _isAttached = true;
        _collider.Disabled = true;
    }

    private void Release()
    {
        _isAttached = false;
        LinearVelocity = Vector2.Zero;
        _collider.Disabled = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_isAttached && @event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && !mouseEvent.Pressed)
        {
            Release();
        }
    }
}