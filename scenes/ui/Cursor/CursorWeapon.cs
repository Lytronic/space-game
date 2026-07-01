using Godot;
using System;

public partial class CursorWeapon : Sprite2D
{
	
	private Cursor _parent;
	private bool _previousModeState = false;

	public override void _Ready()
	{
		_parent = GetParent<Cursor>();
		Visible = _parent.CursorModeWeapon;
		_previousModeState = _parent.CursorModeWeapon;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bool currentMode = _parent.CursorModeWeapon;
		
		if (currentMode != _previousModeState)
		{
			_previousModeState = currentMode;
			Visible = currentMode;
		}
		if (Visible)
		{
			GlobalPosition = GetGlobalMousePosition();
		}
	}
}
