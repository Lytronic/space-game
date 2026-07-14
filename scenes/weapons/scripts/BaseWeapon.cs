using Godot;
using System;

/// <summary>
/// A Node which is responsible for attacking entities by inflicting damage upon them.
/// This base class serves as a common ancestor for all sorts of weapons.
/// </summary>
public partial class BaseWeapon : Node2D
{
	[Export] public float Cooldown;

	public SceneTreeTimer CooldownTimer { get; protected set; }
	
	/// <summary>
	/// Trigger the weapon if its cooldown is finished.
	/// </summary
	/// <param name="direction">The direction in which to shoot relative to the weapons position</param>
	/// <param name="baseDamage">Additional damage which is added to the projectiles damage value</param>
	/// <param name="modifier">Modifier which is the resulting damage is multiplied by</param>
	public virtual void Fire(Vector2 direction, float baseDamage, float modifier) {}

	/// <summary>
	/// Release the weapon trigger.
	/// This is implemented by weapons which continuously shoot until stopped
	/// </summary>
	public virtual void Release() {}
}
