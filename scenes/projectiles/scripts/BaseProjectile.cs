using Godot;

/// <summary>
/// Shared projectile behaviour for player and enemy weapons.
/// </summary>
[GlobalClass]
public partial class BaseProjectile : Area2D
{
	[Export] public float Damage = 1.0f;
	[Export] public float Speed = 420.0f;
	[Export] public Vector2 Direction = Vector2.Right;
	[Export] public bool Malicious = true;
	[Export] public float Lifetime = 3.0f;

	private Node2D _owner;
	private float _remainingLifetime;

	public override void _Ready()
	{
		_remainingLifetime = Lifetime;
		BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		_remainingLifetime -= dt;

		if (_remainingLifetime <= 0.0f)
		{
			QueueFree();
			return;
		}

		Vector2 movement = MoveInPattern(delta);
		if (movement.LengthSquared() > 0.0001f)
		{
			movement = movement.Normalized();
			Rotation = movement.Angle();
		}

		GlobalPosition += movement * Speed * dt;
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body == _owner)
			return;

		bool hitDamageTarget = false;

		if (Malicious && body is Player player)
		{
			player.TakeDamage(Damage);
			hitDamageTarget = true;
		}
		else if (!Malicious && body is BaseEnemy enemy)
		{
			enemy.TakeDamage(Damage);
			hitDamageTarget = true;
		}

		if (hitDamageTarget || body is RigidBody2D || body is StaticBody2D || body is CharacterBody2D)
		{
			QueueFree();
		}
	}

	public void SpawnProjectile(float damage, Vector2 direction)
	{
		Launch(damage, direction, GlobalPosition, Malicious);
	}

	/// <summary>
	/// Places the projectile in the active play space and starts moving it.
	/// </summary>
	public void Launch(float damage, Vector2 direction, Vector2 startPosition, bool isMalicious, Node2D owner = null, float speed = -1.0f)
	{
		Damage = damage;
		Direction = direction.LengthSquared() > 0.0001f ? direction.Normalized() : Vector2.Right;
		Malicious = isMalicious;
		_owner = owner;

		if (speed > 0.0f)
			Speed = speed;

		Node projectileParent = PlayerVariables.Instance.Space;
		if (projectileParent == null && owner != null)
			projectileParent = owner.GetTree()?.CurrentScene;

		projectileParent ??= GetTree()?.CurrentScene;
		if (!IsInsideTree())
		{
			if (projectileParent == null)
			{
				GD.PushWarning("Projectile launched without an active parent; removing it.");
				QueueFree();
				return;
			}

			projectileParent.AddChild(this);
		}
		else if (projectileParent != null && GetParent() != projectileParent)
		{
			Reparent(projectileParent);
		}

		GlobalPosition = startPosition;
		Rotation = Direction.Angle();
	}

	public virtual Vector2 MoveInPattern(double time)
	{
		return Direction;
	}
}
