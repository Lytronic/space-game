using Godot;
using System;

public partial class Plasma : BaseProjectile
{
    // There should be a projectile that cannot be stopped by lasers.
    // Otherwise the Oculox is too powerful.
	private AudioStreamPlayer2D _shootSound;
    /*public override void _Ready()
    {
        _shootSound = GetNode<AudioStreamPlayer2D>("ShootSound");
		_shootSound.Stream = GD.Load<AudioStream>("res://sfx/game/weapons/plasma.mp3");
    }*/

	public override void TakeDamage(float damage)
	{
		return;
	}
}
