using Godot;

/// <summary>
/// Enemy AI that keeps range, strafes, and fires at the player.
/// </summary>
[GlobalClass]
public partial class BaseEnemyAI : Node
{
	private const float MinDistanceSquared = 0.0001f;

	[Export] public float PreferredRange = 260.0f;
	[Export] public float RetreatRange = 145.0f;
	[Export] public float FireRange = 430.0f;
	[Export] public float Acceleration = 520.0f;
	[Export] public float FireCooldown = 1.25f;
	[Export] public float ProjectileSpeed = 330.0f;
	[Export] public float ProjectileSpawnDistance = 30.0f;
	[Export] public float AimSpreadRadians = 0.08f;
	[Export] public float ContactDamageRange = 42.0f;
	[Export] public float ContactDamageCooldown = 0.8f;

	private BaseEnemy _enemy;
	private Player _target;
	private PackedScene _projectileScene;
	private readonly RandomNumberGenerator _rng = new();
	private float _fireTimer;
	private float _contactDamageTimer;
	private int _strafeDirection = 1;

	public override void _Ready()
	{
		_enemy = GetParent<BaseEnemy>();
		_projectileScene = ResourceLoader.Load<PackedScene>("res://scenes/projectiles/scenes/BaseProjectile.tscn");
		_rng.Randomize();
		_fireTimer = _rng.RandfRange(0.1f, Mathf.Max(0.1f, FireCooldown));
		_strafeDirection = _rng.RandiRange(0, 1) == 0 ? -1 : 1;
		AcquireTarget();
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		
		if (_enemy == null || _enemy.IsDead)
			return;

		if (!IsTargetValid())
			AcquireTarget();

		if (!IsTargetValid())
		{
			_enemy.Velocity = _enemy.Velocity.MoveToward(Vector2.Zero, Acceleration * dt);
			_enemy.MoveAndSlide();
			return;
		}

		_fireTimer -= dt;
		_contactDamageTimer -= dt;

		Vector2 toTarget = _target.GlobalPosition - _enemy.GlobalPosition;
		float distance = toTarget.Length();
		if (distance * distance < MinDistanceSquared)
			return;

		Vector2 direction = toTarget / distance;
		UpdateMovement(direction, distance, dt);
		UpdateShooting(direction, distance);
	}

	private void AcquireTarget()
	{
		Node scene = GetTree().CurrentScene;
		_target = scene.GetNode<Player>("Player");
	}

	private bool IsTargetValid()
	{
		return _target != null
			&& !_target.IsQueuedForDeletion()
			&& PlayerVariables.Stats.CurrentHealth > 0.0f;
	}

	private void UpdateMovement(Vector2 direction, float distance, float dt)
	{
		if (_rng.Randf() < 0.01f)
			_strafeDirection *= -1;

		Vector2 desiredVelocity;
		if (distance > PreferredRange)
		{
			desiredVelocity = direction * _enemy.Speed;
		}
		else if (distance < RetreatRange)
		{
			desiredVelocity = -direction * (_enemy.Speed * 0.75f);
		}
		else
		{
			Vector2 strafe = direction.Rotated(Mathf.Pi / 2.0f * _strafeDirection);
			desiredVelocity = strafe * (_enemy.Speed * 0.65f);
		}

		_enemy.Velocity = _enemy.Velocity.MoveToward(desiredVelocity, Acceleration * dt);
		_enemy.MoveAndSlide();

		float targetRotation = direction.Angle() + Mathf.Pi / 2.0f;
		_enemy.Rotation = Mathf.LerpAngle(_enemy.Rotation, targetRotation, Mathf.Clamp(8.0f * dt, 0.0f, 1.0f));
	}

	private void UpdateShooting(Vector2 direction, float distance)
	{
		if (_fireTimer > 0.0f || distance > FireRange || _projectileScene == null)
			return;

		Vector2 aimDirection = direction.Rotated(_rng.RandfRange(-AimSpreadRadians, AimSpreadRadians));
		BaseProjectile projectile = _projectileScene.Instantiate<BaseProjectile>();
		Vector2 spawnPosition = _enemy.GlobalPosition + aimDirection * ProjectileSpawnDistance;
		projectile.Launch(aimDirection, spawnPosition, _enemy);

		_fireTimer = Mathf.Max(0.1f, FireCooldown) * _rng.RandfRange(0.75f, 1.25f);
	}
}
