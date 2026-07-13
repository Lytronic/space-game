using Godot;
using System;

public partial class BaseStation : CharacterBody2D
{
	private AudioStreamPlayer2D _explosionSound;
	[Export] public int Health = 3000;
	[Export] public int Shield = 2000;

	public override void _Ready()
	{
		Rotation = 0;
		_explosionSound = GetNode<AudioStreamPlayer2D>("ExplosionSound");
	}

	public override void _PhysicsProcess(double delta)
	{
		Rotation = Rotation + 0.0001f;
	}
}
