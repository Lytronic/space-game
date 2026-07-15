using Godot;
using System;
using System.Xml.Serialization;

/// <summary>
/// Weapon that casts a ray to where it's aimed and applies continuous damage there.
/// This is used for the Laser and Arc weapons.
/// </summary>
[GlobalClass]
public partial class RaycastWeapon : BaseWeapon
{
	[Export] public float DamagePerSecond;
	[Export] public float EnergyPerSecond;
	[Export] public float Range;
	[Export] public float RayWidth;
	[Export] public float StunTime = 0.0f;
	[Export] public string FiringSoundPath;
	public AudioStreamPlayer2D FiringSoundPlayer;

	// whether the weapon is currently shooting
	private bool _active = false;
	private bool _parentIsPartsManager = false;
	private Vector2 _targetPos;
	private Vector2 _direction;

	private SoundManager _soundManager;

	public override void _Ready()
	{
		CooldownTimer = GetTree().CreateTimer(0.0f);
		if (GetParent() is PartsManager)
		{
			_parentIsPartsManager = true;
		}
		FiringSoundPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
	}

	public override void _Draw()
	{
		if (_active)
		{
			// Draw commands need local space coordinates, so we need to transform our world space coordinates
			// using the inverse of the global transform matrix.
			var transform = GetGlobalTransform().AffineInverse();

			// We could just draw a line using DrawLine(), but it wouldn't have UV coordinates for its shader.
			// This means we need to draw an axis aligned rectangle and rotate it by the correct angle.
			float sign = Mathf.Sign(_direction.Y);
			DrawSetTransformMatrix(transform.Rotated(sign * Mathf.Acos(_direction.Dot(Vector2.Right))));
				
			var rect = new Rect2(GlobalPosition + new Vector2(0.0f, RayWidth / 2),
				(Vector2.Right * (_targetPos - GlobalPosition).Length()) - new Vector2(0.0f, RayWidth));	
			DrawRect(rect, Colors.Red);
		}
	}

	public override void _Process(double delta)
	{
		if (_active)
		{
			if (_parentIsPartsManager)
			{
				float energyUse = EnergyPerSecond * (float)delta;
				if (energyUse > PlayerVariables.Stats.Energy)
				{
					Release();
					return;
				}
				else
				{
					PlayerVariables.Instance.UseEnergy(energyUse);
				}
			}
			
			var spaceState = GetWorld2D().DirectSpaceState;

			Vector2 rayEnd = GlobalPosition + (_direction * Range);

			var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, rayEnd);
			query.Exclude = [ GetParent<PartsManager>().player.GetRid() ];
			query.CollideWithAreas = true;
		
			var result = spaceState.IntersectRay(query);

			if (result.ContainsKey("position"))
			{
				_targetPos = (Vector2)result["position"];

				// Use HasMethod in this case because the collider could be any physics object
				// and not all of them have our TakeDamage() method.
				var target = (GodotObject)result["collider"];
				if (target.HasMethod("TakeDamage"))
				{
					target.Call("TakeDamage", DamagePerSecond * delta);
				}

				if (StunTime > 0 && target.HasMethod("Stun"))
				{
					target.Call("Stun", StunTime);
				}
			}
			else
			{
				_targetPos = rayEnd;
			}

			QueueRedraw();
		}
	}

	public override void Fire(Vector2 direction, float baseDamage, float modifier)
	{
		if (CooldownTimer.TimeLeft > 0)
			return;

		_active = true;
		_direction = direction;
	}

	public override void Release()
	{
		CooldownTimer = GetTree().CreateTimer(Cooldown);
		_active = false;

		QueueRedraw();
	}

	public async void FireSound()
	{
		if(_active)
		{
			if(FiringSoundPlayer.Playing)
			{
				await ToSignal(FiringSoundPlayer, AudioStreamPlayer.SignalName.Finished);
				_playFiringSound();
			}
			else
			{
				_playFiringSound();
			}
		}
	}

	private void _playFiringSound()
	{
		FiringSoundPlayer.Stream = GD.Load<AudioStream>(FiringSoundPath);

		FiringSoundPlayer.Play();
	}
}
