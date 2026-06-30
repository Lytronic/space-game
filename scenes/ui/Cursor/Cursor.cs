using Godot;
using System;

public partial class Cursor : Node2D
{
	// sprite and ref to the player in the inspector
	[Export] public Texture2D CursorTexture;
	[Export] public NodePath PlayerShip;
	
	// ref
	public Sprite2D cursorSprite;
	private Node2D _shipPlayer;
	
	
	public override void _Ready()
	{
		Input.SetMouseMode(Input.MouseModeEnum.Hidden);

		// new cursor object
		cursorSprite = new Sprite2D();
		cursorSprite.Texture = CursorTexture;
		// center pivot point
		cursorSprite.Centered = true; 
		AddChild(cursorSprite);
		
		// player ref
		if (PlayerShip != null)
		{
			_shipPlayer = GetNode<Node2D>(PlayerShip);
		}
	}

	public override void _Process(double delta)
	{
		if (cursorSprite == null) return;

		// mouse position sync (very laggy, but otherwise there could be no rotation with a custom system cursor)
		cursorSprite.GlobalPosition = GetGlobalMousePosition();

		if (_shipPlayer != null)
		{
			// calc the rotation relative to the player ship
			Vector2 direction = (GetGlobalMousePosition() - _shipPlayer.GlobalPosition).Normalized();
			
			// calc the angle the cursor needs to turn
			float angle = (float)Math.Atan2(direction.Y, direction.X);
			// add 90 degrees in radian
			cursorSprite.Rotation = angle + Mathf.Pi / 2f;

		}		
	}
	public override void _ExitTree()
	{
		// when the scene closes we need the cursor back
		Input.SetMouseMode(Input.MouseModeEnum.Visible);
	}	
}
