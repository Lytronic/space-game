using Godot;
using System;

public partial class BaseMissile : BaseProjectile
{
	private GpuParticles2D _explosion;
	private Sprite2D _sprite;

	private GpuParticles2D _engineParticles;
	private SoundManager _soundManager;

	// Sounds
	private AudioStreamPlayer2D _shootSound;
	private AudioStreamPlayer2D _impactSound;

	private bool _exploding = false;

	public override void _Ready()
	{
		base._Ready();
		_impactSound = GetNode<AudioStreamPlayer2D>("ImpactSound");
		_shootSound = GetNode<AudioStreamPlayer2D>("ShootSound");
		_sprite = GetNode<Sprite2D>("BaseProjectileSprite2D");
		_explosion = GetNode<GpuParticles2D>("GPUParticles2D");
		_explosion.Emitting = false;

		_shootSound.Stream = GD.Load<AudioStream>("res://sfx/game/weapons/cannon.mp3");
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
	}

	public override void OnBodyEntered(Node2D body)
	{
		if (body == _owner || _exploding || IsQueuedForDeletion()) return;

		_exploding = true;
		Explode();

		GetTree().CreateTimer(_explosion.Lifetime).Timeout += () => base.OnBodyEntered(body);
	}

	public override void TakeDamage(float damage)
	{
		if (damage > 0)
		{
			Explode();
			GetTree().CreateTimer(_explosion.Lifetime).Timeout += QueueFree;
		}
	}

	public virtual void Explode()
	{
		_impactSound.Stream = GD.Load<AudioStream>("res://sfx/game/enemy/explosion_distant.mp3");
		Speed = 0.0f;
		_sprite.Hide();
		_explosion.OneShot = true;
		_explosion.Restart();
		_explosion.Emitting = true;
		_exploding = true;
		_impactSound.Play();
		
	}
}
