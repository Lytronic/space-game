using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

[GlobalClass]
public partial class BaseEnemy : RigidBody2D
{

	//here is the public loot table that EnemySalvage uses to choose a drop from; any object in here is gonna be unique

	[Export] public NodePath salvagePath = "EnemySalvage";

	//public const int amountOfLoot = 5;
	[Export] public ShipPart[] lootTable { get; set; } 

	//make the enemy have a hitbox and etc

	//the most basic and necessary enemy stats
	public const float scaling = 1.1f;
	public float speed = 1f;
	[Export] public int health = 1;
	[Export] public float damage = 1;
	[Export] public float resistance = 0.1f;




	public override void _EnterTree()
	{
		//here the loot table will be created before all the children are loaded

		//createLootTable();
		//generateDropStats();

	}
	public override void _Ready()
	{
		//complete the loot table with stats
		createLootTable();
		generateDropStats();


		//correctly scale stats
		health = scaleStat(health);
		damage = scaleStat(damage);
		resistance = scaleStat(resistance);

	}

	public override void _Process(double delta)
	{
	
	}

	public virtual void createLootTable()
	{
		// if you don't order these by smallest rarity first, I'm murdering you; the first one must be 'null' unlesss the enemy has guaranteed drops
		lootTable = new[] { null, new DebugMultitool(), new DebugMultitool(), new DebugMultitool(), new DebugMultitool() };
	}
	public void generateDropStats()
	{
		foreach (ShipPart part in lootTable)
		{
			if(part != null)
			{
				part.Initialize();
				part.generateStats();
			}
		}
		//GD.Print("Generated Stats for loot "); //debug
	}

	public void spawnEnemy()
	{

	}
	//make it more difficult according to the danger Level
	public int scaleStat(int stat)
	{
		stat *= (int)Math.Pow(scaling, PlayerVariables.Instance.DangerLevel);
		return stat;
	}
	public float scaleStat(float stat)
	{
		stat *= (float)Math.Pow(scaling, PlayerVariables.Instance.DangerLevel);
		return stat;
	}

	public void enemyTakeDamage(float damage)
	{
		damage -= damage * resistance; //percentual decrease in damage taken 
		health -= (int)damage;

		if(health <= 0 ) enemyDie();
		//GD.Print($"Enemy damaged {damage}"); //debug
	}
	public bool IsLastEnemy()
	{
		var scene = GetTree().CurrentScene;
		if (scene == null) return false;

		foreach (Node child in scene.GetChildren())
		{
			if (child == this) continue;

			if (child is BaseEnemy enemy && !enemy.IsQueuedForDeletion())
				return false;
		}

		return true;
	}
	public void enemyDie()
	{
		GD.Print($"Enemy {this} died");
		//detatch ai script for resurrection
		//delete collisions for resurrection
		GetNode<EnemySalvage>(salvagePath).dropLoot();

		// Check if another enemy exists
		bool levelCleared = IsLastEnemy();

		// Remove node
		QueueFree();

		// Initiate change to build menu scene if this was the last enemy
		if (levelCleared) GetTree().CurrentScene.Call("OpenBuildMenuAfterDelay", 1.5f);
	}
}
