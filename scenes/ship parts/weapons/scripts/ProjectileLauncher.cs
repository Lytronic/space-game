using Godot;
using System;

/// <summary>
/// Weapon that launches projectiles, such as missiles or plasma.
/// See base class documentation.
/// </summary>
[GlobalClass]
public partial class ProjectileLauncher : BaseWeapon
{
	[ExportGroup("Projectiles")]
	[Export(PropertyHint.File, "*.tscn")]
	public string ProjectileScene;

	[Export] public int Count = 1;
	[Export] public float Spread = 0.0f;

	private PackedScene _projectileScene;

	public override void _Ready()
	{
		CooldownTimer = GetTree().CreateTimer(Cooldown);
		_projectileScene = ResourceLoader.Load<PackedScene>(ProjectileScene);
	}

	public override void Fire(Vector2 direction, float baseDamage, float modifier)
	{
		if (CooldownTimer.TimeLeft > 0)
			return;

		var parent = GetParent<Node2D>();

		for (int i = 1; i <= Count; i++)
		{
			BaseProjectile projectile = _projectileScene.Instantiate<BaseProjectile>();
			projectile.Launch((projectile.Damage + baseDamage) * modifier, projectile.Speed, direction.Rotated((-(Spread / Count) + i * (Spread / Count)) / Mathf.Tau), GlobalPosition, parent is not Player, parent);
		}
		
		CooldownTimer = GetTree().CreateTimer(Cooldown);
	}
}
