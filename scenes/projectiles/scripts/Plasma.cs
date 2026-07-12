using Godot;
using System;

public partial class Plasma : BaseProjectile
{
	// There should be a projectile that cannot be stopped by lasers.
	// Otherwise the Oculox is too powerful.
	public override void TakeDamage(float damage)
	{
		return;
	}
}
