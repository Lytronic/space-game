using Godot;
using System;

/// <summary>
/// Core combat, loot, score, and wave-clear behavior for ship enemies.
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

	public bool IsDead { get; private set; } = false;

	[Signal]
	public delegate void KilledEventHandler();

	private GpuParticles2D _explosion;
	private Sprite2D _sprite;
	private CollisionShape2D _collisionShape;

	public override void _Ready()
	{
		_explosion = GetNode<GpuParticles2D>("ExplosionParticle");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		
		if (lootTable == null || lootTable.Length == 0)
			CreateLootTable();

		GenerateDropStats();
		ScaleStatsForCurrentDanger();
	}

	public virtual void CreateLootTable()
	{
		// if you don't order these by smallest rarity first, I'm murdering you; the first one must be 'null' unlesss the enemy has guaranteed drops
		lootTable = new[] { null, new DebugMultitool(), new DebugMultitool(), new DebugMultitool(), new DebugMultitool() };
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

	public bool IsLastEnemy()
	{
		var scene = GetTree().CurrentScene;
		if (scene == null) return false;

		return !HasOtherLivingEnemy(scene);
	}

	private bool HasOtherLivingEnemy(Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			if (child != this && child is BaseEnemy enemy && !enemy.IsDead && !enemy.IsQueuedForDeletion())
				return true;

			if (HasOtherLivingEnemy(child))
				return true;
		}

		return false;
	}

	public void Die()
	{
		EmitSignal(SignalName.Killed);
		
		if (IsDead)
			return;

		IsDead = true;
		PlayerVariables.Stats.Score += ScoreValue;

		GetNodeOrNull<EnemySalvage>(salvagePath)?.dropLoot();

		bool levelCleared = IsLastEnemy();

		Explode();
		GetTree().CreateTimer(_explosion.Lifetime).Timeout += () => QueueFree();

		if (levelCleared)
		{
			PlayerVariables.Instance.ChangeDifficulty(1);

			Node currentScene = GetTree().CurrentScene;
			if (currentScene?.HasMethod("OpenBuildMenuAfterDelay") == true)
				currentScene.Call("OpenBuildMenuAfterDelay", 1.5f);
		}
	}

	public virtual void Explode()
	{
		_sprite.Hide();
		_collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
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
}
