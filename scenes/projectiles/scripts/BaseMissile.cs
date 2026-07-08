using Godot;
using System;

public partial class BaseMissile : BaseProjectile
{
	private GpuParticles2D _explosion;
	private Sprite2D _sprite;
	private SoundManager _soundManager;
	private bool _exploding = false;

	public override void _Ready()
	{
		base._Ready();

		_sprite = GetNode<Sprite2D>("BaseProjectileSprite2D");
		_explosion = GetNode<GpuParticles2D>("GPUParticles2D");
		_explosion.Emitting = false;

		_soundManager = GetNode<SoundManager>("/root/SoundManager");
	}

	public override void OnBodyEntered(Node2D body)
	{
		if (body == _owner) return;
		if (_exploding) return;

		_exploding = true;
		Explode();

		GetTree().CreateTimer(_explosion.Lifetime).Timeout += () => base.OnBodyEntered(body);
	}

	public virtual void Explode()
	{
		Speed = 0.0f;
		_sprite.Hide();
		_explosion.OneShot = true;
		_explosion.Restart();
		_explosion.Emitting = true;
	}
}
