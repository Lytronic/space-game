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
	[Export] public bool Reflectable = false;
	public AudioStreamPlayer2D FiringSoundPlayer;

	// whether the weapon is currently shooting
	private bool _active = false;
	private bool _reflecting = false;
	private Node _parent;
	private Node2D _reflector;
	private Vector2 _targetPos;
	private Vector2 _reflectionPos;
	private Vector2 _direction;
	private Vector2 _reflectingDirection;

	private SoundManager _soundManager;

	public override void _Ready()
	{
		CooldownTimer = GetTree().CreateTimer(0.0f);
		FiringSoundPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
		_parent = GetParent();
	}

	public override void _Draw()
	{
		// Draw commands need local space coordinates, so we need to transform our world space coordinates
		// using custom transformation matrices.
		// We could just draw a line using DrawLine(), but it wouldn't have UV coordinates for its shader.
		// This means we need to draw an axis aligned rectangle and rotate it by the correct angle for each ray.
		if (_active)
	    {
	        var weaponInverse = GetGlobalTransform().AffineInverse();

	        // Primary ray
	        float angle1 = Mathf.Sign(_direction.Y) * Mathf.Acos(_direction.Dot(Vector2.Right));
	        var worldXform1 = new Transform2D(angle1, GlobalPosition);
	        DrawSetTransformMatrix(weaponInverse * worldXform1);

	        float length1 = (_targetPos - GlobalPosition).Length();
	        var rect1 = new Rect2(0.0f, -RayWidth / 2, length1, RayWidth);
	        DrawRect(rect1, Colors.Red);

	        if (_reflecting)
	        {
	            // Reflected ray - origin is the reflector, not the weapon
	            float angle2 = Mathf.Sign(_reflectingDirection.Y) * Mathf.Acos(_reflectingDirection.Dot(Vector2.Right));
	            var worldXform2 = new Transform2D(angle2, _targetPos);
	            DrawSetTransformMatrix(weaponInverse * worldXform2);

	            float length2 = (_reflectionPos - _targetPos).Length(); 
	            var rect2 = new Rect2(0.0f, -RayWidth / 2, length2, RayWidth);
	            DrawRect(rect2, Colors.Red);
	        }
	    }
	}

	public override void _Process(double delta)
	{
		if (_active)
		{
			if (_parent is PartsManager)
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
			query.Exclude = [_parent is PartsManager pm ? pm.player.GetRid() : ((CollisionObject2D)_parent).GetRid()];
			query.CollideWithAreas = true;

			var result = spaceState.IntersectRay(query);

			if (result.ContainsKey("position"))
			{
				_targetPos = (Vector2)result["position"];
				var target = (GodotObject)result["collider"];

				// render a reflected ray if the player has a mirror
				if (target is Player player && PlayerVariables.Instance.HasPart<Mirror>() && Reflectable)
				{
					_reflecting = true;
					_reflectingDirection = (-_direction).Reflect((Vector2)result["normal"]); 
					_reflector = player;
					rayEnd = _targetPos + _reflectingDirection * Range;
				    query.From = _targetPos;   
					query.To = rayEnd;
					query.Exclude = [ player.GetRid() ];
					result = spaceState.IntersectRay(query);


					if (result.ContainsKey("position"))
					{
						_reflectionPos = (Vector2)result["position"];

						// the new damage target is what the second ray hits
						target = (GodotObject)result["collider"];
					}
					else
					{
						_reflectionPos = rayEnd;
						goto SkipDamage;
					}
				}
				else
				{
					_reflecting = false;
				}

				// Use HasMethod in this case because the collider could be any physics object
				// and not all of them have our TakeDamage() method.				
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
		SkipDamage:

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
		if (_active)
		{
			if (FiringSoundPlayer.Playing)
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
