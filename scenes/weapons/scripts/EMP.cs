using Godot;
using System;

/// <summary>
/// Electromagnetic pulse: a weapon that stuns all enemies within a radius
/// by interfering with their hardware.
/// </summary>
[GlobalClass]
public partial class EMP : BaseWeapon
{
	[Export] public float Radius = 100.0f;
	[Export] public float StunTime = 10.0f;

	public override void Fire(Vector2 direction, float baseDamage, float modifier)
	{
		var parent = GetParent();

		if (parent is Player)
		{
			if (EnergyOnUse > PlayerVariables.Stats.Energy
				|| FuelOnUse > PlayerVariables.Stats.Fuel)
			{
				return;
			}
			else
			{
				PlayerVariables.Instance.UseEnergy(EnergyOnUse);
				PlayerVariables.Instance.UseFuel(FuelOnUse);
			}
		}
		
		foreach (var entity in PlayerVariables.Space.GetChildren())
		{
			if (parent is BaseEnemy && entity is Player player)
			{
				player.Stun(StunTime);
				GD.Print("stunnedPplayer");
			}
			else if (parent is PartsManager && entity is BaseEnemy enemy)
			{
				enemy.Stun(StunTime);

			}
		}
	}
}
