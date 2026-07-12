using System;
using System.Linq;
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
	
	/// <value>How long to hold a selected weapon before switching to another one.</value>
	[Export] public float WeaponHoldTime = 0.0f;

	// these are arrays so we can have multiple weapons of the same type fired concurrently
	// (e.g. 1 rocket launcher on each side of the enemy ship)
	[Export] public Array<BaseWeapon> Primary;
	[Export] public Array<BaseWeapon> Secondary;
	[Export] public Array<BaseWeapon> Tertiary;

	private BaseEnemy _enemy;
	private Player _target;
	private PackedScene _projectileScene;
	private readonly RandomNumberGenerator _rng = new();
	private int _strafeDirection = 1;

	private Array<BaseWeapon> _currentWeapon;
	private SceneTreeTimer _weaponHoldTimer;

	public override void _Ready()
	{
		_enemy = GetParent<BaseEnemy>();
		_rng.Randomize();
		_strafeDirection = _rng.RandiRange(0, 1) == 0 ? -1 : 1;
		AcquireTarget();
		_currentWeapon = Primary;
		_weaponHoldTimer = GetTree().CreateTimer(0.0f);
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
		if (_enemy.StunTimer.TimeLeft > 0) return;
		
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
		if (_enemy.StunTimer.TimeLeft > 0) return;

		if (distance > FireRange) return;

		if (_weaponHoldTimer.TimeLeft == 0)
		{
			ForEachWeapon(_currentWeapon, weapon => weapon.Release());
			SelectWeapon();
		}

		ForEachWeapon(_currentWeapon, weapon =>	weapon.Fire((_target.GlobalPosition - weapon.GlobalPosition).Normalized(), _enemy.Damage, 1.0f));
	}

	/// <summary>
	/// Select whether to use the primary, secondary, or tertiary weapon until this method is called again.
	/// Fall back to the primary if the selected one is still on cooldown.
	/// By default, this is random. Override to add custom behaviour.
	/// </summary>
	protected virtual void SelectWeapon()
	{
		_currentWeapon = Primary;
		
		switch (_rng.RandiRange(0, 2))
		{
			case 0:
				_currentWeapon = Primary;
				break;
			case 1:
				if (Secondary == null || ForEachWeapon(Secondary, weapon => weapon.CooldownTimer.TimeLeft).Min() > 0) break;
				_currentWeapon = Secondary;
				break;
			case 2:
				if (Tertiary == null || ForEachWeapon(Tertiary, weapon => weapon.CooldownTimer.TimeLeft).Min() > 0) break;
				_currentWeapon = Tertiary;
				break;
		}

		_weaponHoldTimer = GetTree().CreateTimer(WeaponHoldTime);
	}

	/// <summary>
	/// Helper to map a void function to all weapons in an array.
	/// For some reason, Godot arrays don't have this by default.
	/// </summary>
	protected static void ForEachWeapon(Array<BaseWeapon> weapons, Action<BaseWeapon> f)
	{
		foreach (var weapon in weapons)
		{
			f(weapon);
		}	
	}
	
	/// <summary>
	/// Helper to map a function to all weapons in an array and return the resulting array.
	/// For some reason, Godot arrays don't have this by default.
	/// </summary>
	protected static Array<T> ForEachWeapon<[MustBeVariant] T>(Array<BaseWeapon> weapons, Func<BaseWeapon, T> f)
	{
		Array<T> ret = [];
		foreach (var weapon in weapons)
		{
			ret.Add(f(weapon));
		}

		return ret;
	}
}
