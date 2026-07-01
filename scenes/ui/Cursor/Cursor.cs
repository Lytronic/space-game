using Godot;
using System;

public partial class Cursor : Node2D
{

	[Export] public NodePath PlayerShip;

	public bool CursorModeWeapon { get; private set; } = false;
	
	public override void _Ready()
	{
		Input.SetMouseMode(Input.MouseModeEnum.Hidden);


	}

	public override void _Process(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Right))
		{
			CursorModeWeapon = true;
		}
		else
		{
			CursorModeWeapon = false;
		}
	}

	public override void _ExitTree()
	{
		// when the scene closes we need the cursor back
		Input.SetMouseMode(Input.MouseModeEnum.Visible);
	}	
}
