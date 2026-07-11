using Godot;
using System;

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

	// whether the weapon is currently shooting
	private bool _active = false;
	// whether we need to process it this frame
	private bool _process = false;
	private Vector2 _targetPos;
	private Vector2 _direction;

	public override void _Ready()
	{
		CooldownTimer = GetTree().CreateTimer(0.0f);
	}

	public override void _Draw()
	{
		if (_active)
		{
			// Draw commands need local space coordinates, so we need to transform our world space coordinates
			// using the inverse of the global transform matrix.
			var transform = GetGlobalTransform().AffineInverse();

			DrawSetTransformMatrix(transform);
			DrawLine(GlobalPosition, _targetPos, Colors.Red, 1.0f, true);
		}
	}

	public override void _Process(double delta)
	{
		if (_active)
		{
			var spaceState = GetWorld2D().DirectSpaceState;

			Vector2 rayEnd = _direction * Range;

			var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, rayEnd);
			query.Exclude = [ GetParent<CollisionObject2D>().GetRid() ];
		
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
}
