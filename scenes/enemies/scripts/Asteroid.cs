using Godot;
using System;

public partial class Asteroid : RigidBody2D
{
	private SoundManager _soundManager;

	private AudioStreamPlayer2D _explosionSound;
	private Sprite2D _asteroidSprite;
	private GpuParticles2D _explosionParticles;
	public float Health = 1000;

    public override void _Ready()
    {
		_explosionParticles = GetNode<GpuParticles2D>("ExplosionParticle");
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
		_explosionSound = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        _asteroidSprite = GetChild<Sprite2D>(0);
		_explosionParticles.Hide();
		

    }

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
		GD.Print("Dying!");
		Explode();
		//GetTree().CreateTimer(5.91f).Timeout = Remove();
	}
	
	private void Explode()
	{
		GD.Print("Exploding!");
		_asteroidSprite.Hide();
		_explosionParticles.Texture = _asteroidSprite.Texture;
		_explosionParticles.Show();
		_explosionParticles.OneShot = true;
		_explosionParticles.Restart();
		_explosionParticles.Emitting = true;
		PlayExplosionSound();
	}
	private void PlayExplosionSound()
	{
		_explosionSound.Stream = GD.Load<AudioStream>("res://sfx/game/enemy/explosion_distant.mp3");
		GD.Print(_explosionSound.Stream);
		_explosionSound.Play();
	}

	private void Remove()
	{
		QueueFree();
	}
}
