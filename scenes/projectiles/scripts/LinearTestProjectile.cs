using Godot;
using System;

public partial class LinearTestProjectile : BaseProjectile
{
	
	public override void _Ready()
	{
		Speed = 3;
		Direction = Vector2.Right;
		Damage = 99;
	}
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += MoveInPattern(delta) * Speed;
	}

	public override Vector2 MoveInPattern(double time)
	{
		return Direction * (float)time;
	}
}
