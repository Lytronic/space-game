using System;
using System.Linq;
using Godot;
using Godot.Collections;
public partial class BaseWarshipAi : Node
{
	private readonly RandomNumberGenerator _rng = new();
	private Sprite2D[] _mobileCannons;
	private BaseWarship _boss;

	[Export] public float Acceleration = 520.0f;
	[Export] public float FireRange = 430.0f;

	[ExportCategory("Movement")]

	[Export] public Array<BaseWeapon> Primary;
	[Export] public Array<BaseWeapon> Secondary;
	[Export] public Array<BaseWeapon> Tertiary;
	
	[ExportCategory("Weapons")]

	private Player _target;

	[Export] private int _mobileCannonsNumber;

	private Array<BaseWeapon> _currentWeapon;
	public override void _Ready()
	{
		_boss = GetParent<BaseWarship>();
		_mobileCannons = new Sprite2D[_mobileCannonsNumber];
		for(int i = 0; i > _mobileCannons.Length; i++)
		{
			_mobileCannons[i] = GetChild<Sprite2D>(i);
		}
		_rng.Randomize();
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		
		if (_boss == null || _boss.IsDead)
			return;

		if (!IsTargetValid())
			AcquireTarget();

	}
	public override void _Process(double delta)
	{
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
	/*private void FireWeapons(float distance)
	{

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

	protected static void ForEachWeapon(Array<BaseWeapon> weapons, Action<BaseWeapon> f)
	{
		foreach (var weapon in weapons)
		{
			f(weapon);
		}	
	}*/
}
