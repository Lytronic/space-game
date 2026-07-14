using Godot;
using System;

public partial class BaseStation : CharacterBody2D
{
	private AudioStreamPlayer2D _explosionSound;
	[Export] public int Health = 3000;
	[Export] public int Shield = 2000;

	public override void _Ready()
	{
		_explosionSound = GetNode<AudioStreamPlayer2D>("ExplosionSound");
	}

}
