using Godot;
using System;

public partial class Asteroid : RigidBody2D
{
	private SoundManager _soundManager;

	private AudioStreamPlayer2D _explosionSound;
	private Sprite2D _asteroidSprite;
	private GpuParticles2D _explosionParticles;
	private bool _alive;
	public float Health = 1000;
	private CollisionShape2D _collider;

    public override void _Ready()
    {
		_explosionParticles = GetNode<GpuParticles2D>("ExplosionParticle");
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
		_explosionSound = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        _asteroidSprite = GetChild<Sprite2D>(0);
		_explosionParticles.Hide();
		_alive = true;
		_collider = GetNode<CollisionShape2D>("CollisionShape2D");
		_collider.Disabled = false;
    }

	public override void _Process(double delta)
	{
		if (Health <= 0.0f) 
		{
		if(_alive)
			{
				Die(); // if health drops below 0 and the asteroid is still alive, die
			}
		}
	}

	public void TakeDamage(float amount)
	{
		Health -= amount;
	}

	private void Die() // function to initiate the asteroid's death sequence
	{
		_alive = false;
		Explode();
	}
	
	private async void Explode() // Explosion sequence initiation
	{
		_asteroidSprite.Hide();
		_collider.Disabled = true;
		_explosionParticles.Texture = _asteroidSprite.Texture;
		_explosionParticles.Show();
		_explosionParticles.OneShot = true;
		_explosionParticles.Restart();
		_explosionParticles.Emitting = true;
		PlayExplosionSound();
		await ToSignal(_explosionSound, AudioStreamPlayer.SignalName.Finished);
		Remove();
	}
	private void PlayExplosionSound() // self explanatory
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
