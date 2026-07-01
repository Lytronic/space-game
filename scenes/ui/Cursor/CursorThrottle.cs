using Godot;
using System;

public partial class CursorThrottle : Sprite2D
{
	private Node2D _shipPlayer;
	private Cursor _parent;
	
	private Vector2 _currentOffset = Vector2.Zero;
	
	private bool _previousModeState = false;
	
	[Export] public Texture2D SpriteThrottle;
	[Export] public Texture2D SpriteWeaponThrottle;

	public override void _Ready()
	{
		_parent = GetParent<Cursor>();
		NodePath _shipPlayerPath = (NodePath)_parent.Get("PlayerShip");
		_shipPlayer = _parent.GetNode(_shipPlayerPath) as Node2D;
		
		_previousModeState = _parent.CursorModeWeapon;
		UpdateSpriteVisuals(_previousModeState);
		
		if (_previousModeState && _shipPlayer != null)
		{
			_currentOffset = GlobalPosition - _shipPlayer.GlobalPosition;
		}
	}

	public override void _Process(double delta)
	{
		bool currentMode = _parent.CursorModeWeapon;

		if (currentMode != _previousModeState)
		{
			_previousModeState = currentMode;
			UpdateSpriteVisuals(currentMode);
			
			if (currentMode)
			{
				_currentOffset = GlobalPosition - _shipPlayer.GlobalPosition;
			}
		}
		if(currentMode)
		{
			GlobalPosition = _shipPlayer.GlobalPosition + _currentOffset;
		}
		else
		{
			// calc the rotation relative to the ship while following mouse
			GlobalPosition = GetGlobalMousePosition();
			Vector2 direction = (GetGlobalMousePosition() - _shipPlayer.GlobalPosition).Normalized();
			float angle = (float)Math.Atan2(direction.Y, direction.X);
			Rotation = angle + Mathf.Pi / 2f;
		}
	}
		private void UpdateSpriteVisuals(bool isWeaponMode)
	{
		if (isWeaponMode)
		{
		
			if (SpriteWeaponThrottle != null) Texture = SpriteWeaponThrottle;
		
		}
		else
		{
		
			if (SpriteThrottle != null) Texture = SpriteThrottle;
		
		}
	}
}
