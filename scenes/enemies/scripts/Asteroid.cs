using Godot;
using System;

public partial class Asteroid : RigidBody2D
{
	public float Health = 1000;

	public override void _Process(double delta)
	{
		if (Health <= 0.0f) Die();
	}

	public void TakeDamage(float amount)
	{
		Health -= amount;
	}

	private void Die()
	{
		QueueFree();
	}
}
