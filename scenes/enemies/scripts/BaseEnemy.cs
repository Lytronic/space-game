using Godot;
using System;
using System.Linq;

/// <summary>
/// Enemy Node with stats, loot and accompanying methods.
/// Behaviour is managed by BaseEnemyAI nodes.
/// </summary>
[GlobalClass]
public partial class BaseEnemy : CharacterBody2D
{
	//here is the public loot table that EnemySalvage uses to choose a drop from; any object in here is gonna be unique
	[Export] public NodePath salvagePath = "EnemySalvage";
	[Export] public ShipPart[] lootTable { get; set; }

	public const float ScalingPerDangerLevel = 1.1f;

	//the most basic and necessary enemy stats
	[Export] public float Speed = 125.0f;
	[Export] public int Health = 30;
	[Export] public float Damage = 8.0f;
	[Export] public float Resistance = 0.1f;
	[Export] public int ScoreValue = 100;
	[Export] public int MinimumRound = 0;

	public SceneTreeTimer StunTimer;

	private AudioStreamPlayer2D _explosionSound;
	public bool IsDead { get; private set; } = false;

	[Signal]
	public delegate void KilledEventHandler();

	private GpuParticles2D _explosion;
	private Sprite2D _sprite;
	private CollisionPolygon2D _collisionPolygon;

	private SoundManager _soundManager;

	public override void _Ready()
	{
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
		_explosionSound = GetNode<AudioStreamPlayer2D>("ExplosionSound2D");
		_explosion = GetNode<GpuParticles2D>("ExplosionParticle");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_collisionPolygon = GetNode<CollisionPolygon2D>("CollisionPolygon2D");
		StunTimer = GetTree().CreateTimer(0.0f);
		
		if (lootTable == null || lootTable.Length == 0)
			CreateLootTable();

		GenerateDropStats();
		ScaleStatsForCurrentDanger();

		RescueFromAsteroid();
	}

	public virtual void CreateLootTable()
	{
		// if you don't order these by smallest rarity first, I'm murdering you; the first one must be 'null' unlesss the enemy has guaranteed drops
		lootTable = [null, new DebugMultitool(), new DebugMultitool(), new DebugMultitool(), new DebugMultitool(), new Mirror()];
	}

	public void GenerateDropStats()
	{
		foreach (ShipPart part in lootTable)
		{
			if (part != null)
			{
				part.Initialize();
				part.generateStats();
			}
		}
	}

	public int ScaleStat(int stat)
	{
		int danger = Mathf.Max(1, PlayerVariables.Stats.DangerLevel);
		return Mathf.RoundToInt(stat * (float)Math.Pow(ScalingPerDangerLevel, danger - 1));
	}

	public float ScaleStat(float stat)
	{
		int danger = Mathf.Max(1, PlayerVariables.Stats.DangerLevel);
		return stat * (float)Math.Pow(ScalingPerDangerLevel, danger - 1);
	}

	public void TakeDamage(float damage)
	{
		if (IsDead || damage <= 0.0f)
			return;

		float mitigatedDamage = damage * (1.0f - Mathf.Clamp(Resistance, 0.0f, 0.95f));
		int finalDamage = Mathf.Max(1, Mathf.CeilToInt(mitigatedDamage));
		Health -= finalDamage;

		if (Health <= 0)
			Die();
	}

	/// <summary>
	/// Revoke the enemy's ability to move for the given duration.
	/// </summary>
	public void Stun(float duration)
	{
		StunTimer = GetTree().CreateTimer(duration);
	}

	public void Die()
	{
		EmitSignal(SignalName.Killed);
		
		if (IsDead)
			return;

		IsDead = true;
		PlayerVariables.Stats.Score += ScoreValue;

		GetNodeOrNull<EnemySalvage>(salvagePath)?.dropLoot();

		Explode();
		GetTree().CreateTimer(_explosion.Lifetime).Timeout += () => QueueFree();
	}

	public virtual void Explode()
	{
		foreach (var child in GetChildren())
		{
			if (child.HasMethod(CanvasItem.MethodName.Hide))
			{
				child.Call(CanvasItem.MethodName.Hide);
			}
		}
		_explosion.Show();
		_explosionSound.VolumeDb = 0;
		_explosionSound.VolumeLinear *= _soundManager.masterVolume / 100;
		_explosionSound.Stream = GD.Load<AudioStream>("res://sfx/game/enemy/explosion_distant.mp3");
		_explosionSound.Play();
		_collisionPolygon.SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);
		_explosion.OneShot = true;
		_explosion.Restart();
		_explosion.Emitting = true;
	}

	private void ScaleStatsForCurrentDanger()
	{
		Health = Mathf.Max(1, ScaleStat(Health));
		Damage = ScaleStat(Damage);
		Speed = ScaleStat(Speed);
		Resistance = Mathf.Clamp(ScaleStat(Resistance), 0.0f, 0.85f);
		ScoreValue = Mathf.Max(1, ScaleStat(ScoreValue));
	}

	/// <summary
	/// Teleport the enemy if it spawns inside an asteroid.
	/// </summary>
	private void RescueFromAsteroid()
	{
		var spaceState = GetWorld2D().DirectSpaceState;

		var segments = _collisionPolygon.Polygon;

		// Godot throws an exception if given an odd length array for this
		if ((segments.Length % 2) > 0)
		{
			Array.Resize<Vector2>(ref segments, segments.Length + 1);
			segments[^1] = segments[^2];
		}

		var query = new PhysicsShapeQueryParameters2D()
		{
			Shape = new ConcavePolygonShape2D() { Segments = segments },
		};

		var result = spaceState.IntersectShape(query);

		while (result.Count > 0)
		{
			GlobalPosition += GlobalPosition.Normalized() * 10.0f;
			result = spaceState.IntersectShape(query);
		}
	}
}
