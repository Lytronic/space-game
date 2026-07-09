using Godot;
using Godot.Collections;

/// <summary>
/// Enemy AI that keeps range, strafes, and fires at the player.
/// </summary>
[GlobalClass]
public partial class BaseEnemyAI : Node
{
	private const float MinDistanceSquared = 0.0001f;

	[ExportCategory("Movement")]
	[Export] public float PreferredRange = 260.0f;
	[Export] public float RetreatRange = 145.0f;
	[Export] public float FireRange = 430.0f;
	[Export] public float Acceleration = 520.0f;

	[ExportCategory("Weapons")]
	[Export] public Array<BaseWeapon> Primary;
	[Export] public Array<BaseWeapon> Secondary;
	[Export] public Array<BaseWeapon> Tertiary;

	private BaseEnemy _enemy;
	private Player _target;
	private PackedScene _projectileScene;
	private readonly RandomNumberGenerator _rng = new();
	private int _strafeDirection = 1;

	public override void _Ready()
	{
		_enemy = GetParent<BaseEnemy>();
		_rng.Randomize();
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

		Vector2 toTarget = _target.GlobalPosition - _enemy.GlobalPosition;
		float distance = toTarget.Length();
		if (distance * distance < MinDistanceSquared)
			return;

		Vector2 direction = toTarget / distance;
		UpdateMovement(direction, distance, dt);
		FireWeapons(distance);
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

	private void FireWeapons(float distance)
	{
		if (distance > FireRange) return;

		// try to fire a random weapon each time this is called
		Array<BaseWeapon> currentWeapon = Primary;
		
		switch (_rng.RandiRange(0, 2))
		{
			case 0:
				currentWeapon = Primary;
				break;
			case 1:
				if (Secondary == null) break;
				currentWeapon = Secondary;
				break;
			case 2:
				if (Tertiary == null) break;
				currentWeapon = Tertiary;
				break;
		}
		
		foreach (var weapon in currentWeapon)
		{
			if (weapon.CooldownTimer.TimeLeft > 0) continue;

			weapon.Fire((_target.GlobalPosition - weapon.GlobalPosition).Normalized(), _enemy.Damage, 1.0f);
		}
	}
}
