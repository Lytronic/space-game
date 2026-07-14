using System;
using Godot;

/// <summary>
/// Shared projectile behaviour for player and enemy weapons.
/// </summary>
[GlobalClass]
public partial class BaseProjectile : Area2D
{
	[Export] public float Damage = 1.0f;
	[Export] public float Speed = 420.0f;
	[Export] public Vector2 Direction = Vector2.Right;
	[Export] public bool Malicious = true;
	[Export] public float Lifetime = 3.0f;

	[Export] public string LaunchSound;
	[Export] public string ImpactSound;

	protected Node2D _owner;
	private float _remainingLifetime;

	private AudioStreamPlayer2D _shootSound;

	private SoundManager _soundManager;

	public override void _Ready()
	{
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
		_remainingLifetime = Lifetime;
		BodyEntered += OnBodyEntered;
		_shootSound = GetNode<AudioStreamPlayer2D>("ShootSound");
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		_remainingLifetime -= dt;

		if (_remainingLifetime <= 0.0f)
		{
			QueueFree();
			return;
		}

		Vector2 movement = MoveInPattern(delta);
		if (movement.LengthSquared() > 0.0001f)
		{
			movement = movement.Normalized();
			Rotation = movement.Angle();
		}

		GlobalPosition += movement * Speed * dt;
	}

	public virtual void OnBodyEntered(Node2D body)
	{
		if (body == _owner)
			return;

		if (Malicious && body is Player player)
		{
			player.TakeDamage(Damage);
		}
		else if (!Malicious && body is BaseEnemy enemy)
		{
			enemy.TakeDamage(Damage);
		}

		try
		{
			if (!IsQueuedForDeletion())
			{
				QueueFree();
			}
		}
		catch (ObjectDisposedException)
		{
			// This exception means the object has already been deleted elsewhere.
			// We can't synchronize the deletion processes so we just catch this.
			return;
		}
	}

	/// <summary>
	/// Places the projectile in the active play space and starts moving it.
	/// Use this overload for the default damage and speed values of the
	/// relevant subclass.
	/// </summary>
	public virtual void Launch(Vector2 direction, Vector2 startPosition, Node2D owner)
	{
		Launch(Damage, Speed, direction, startPosition, Malicious, owner);
	}

	/// <summary>
	/// Places the projectile in the active play space and starts moving it.
	/// This overload allows for customising damage but overrides possible
	/// adjustments made by subclasses.
	/// These may be modified and passed as parameters.
	/// </summary>
	public virtual void Launch(float damage, float speed, Vector2 direction, Vector2 startPosition, bool isMalicious, Node2D owner)
	{
		Damage = damage;
		Direction = direction.LengthSquared() > 0.0001f ? direction.Normalized() : Vector2.Right;
		Malicious = isMalicious;
		_owner = owner;

		Node projectileParent = PlayerVariables.Space;
		if (projectileParent == null && owner != null)
			projectileParent = owner.GetTree()?.CurrentScene;

		projectileParent ??= GetTree()?.CurrentScene;
		if (!IsInsideTree())
		{
			if (projectileParent == null)
			{
				GD.PushWarning("Projectile launched without an active parent; removing it.");
				QueueFree();
				return;
			}

			projectileParent.AddChild(this);
		}
		else if (projectileParent != null && GetParent() != projectileParent)
		{
			Reparent(projectileParent);
		}

		GlobalPosition = startPosition;
		Rotation = Direction.Angle();
		_shootSound.Stream = GD.Load<AudioStream>(LaunchSound);
		_shootSound.VolumeDb = 0;
		_shootSound.VolumeLinear *= _soundManager.masterVolume / 100;
		_shootSound.Play();
	}

	/// <summary>
	/// Damage the projectile. Since BaseProjectiles have no health value, kill it instantly.
	/// </summary>
	public virtual void TakeDamage(float damage)
	{
		if (damage > 0)
			QueueFree();
	}

	public virtual Vector2 MoveInPattern(double time)
	{
		return Direction;
	}
}
