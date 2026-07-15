using Godot;
using System;

/// <summary>
/// Electromagnetic pulse: a weapon that stuns all enemies within a radius
/// by interfering with their hardware.
/// </summary>
[GlobalClass]
public partial class EMP : BaseWeapon
{
	private GpuParticles2D _pulseParticle;
	private AudioStreamPlayer2D _pulseSound;
	[Export] public float Radius = 100.0f;
	[Export] public float StunTime = 10.0f;

	private SoundManager _soundManager;
	public override void Fire(Vector2 direction, float baseDamage, float modifier)
	{
		_pulseSound = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
		_pulseParticle = GetNode<GpuParticles2D>("GPUParticles2D");
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
		_pulseParticle.Show();
		_pulseParticle.OneShot = true;
		_pulseParticle.Restart();
		_pulseParticle.Emitting = true;
		_pulseSound.Stream = GD.Load<AudioStream>("res://sfx/game/weapons/emp.mp3");
		_pulseSound.VolumeDb = 0;
		_pulseSound.VolumeLinear *= _soundManager.masterVolume / 100;
		_pulseSound.Play();
	}
}
