using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Game : Node2D
{
	private const string AsteroidScenePath = "res://scenes/enemies/scenes/Asteroid.tscn";

	[Export] public int AsteroidCount;

	// This should show up as an array of selectable files in the editor
	// but it doesn't, which is a bug outside our control:
	// https://github.com/godotengine/godot-docs/issues/11655
	//
	// As long as this isn't fixed, we need to paste the paths manually like cave men.
	[Export(PropertyHint.File, "*.tscn")]
	public Array<string> Enemies;

	private PackedScene _asteroidScene;
	private Array<PackedScene> _enemyScenes = [];

	private Node _soundManager;
	private int _enemiesSpawned = 0;
	private int _enemyCount = 0;

	private RandomNumberGenerator _rng;

	private ProgressBar _progressBar;

	public override void _Ready()
	{
		_soundManager = GetNode("/root/SoundManager");
		_rng = new();
		_rng.Randomize();

		PlayerVariables.Space = GetNode<EntityManager>("EntityManager");

		_progressBar = GetNode<ProgressBar>("CanvasLayer/HUD/RoundIndicator/ProgressBar");

		_asteroidScene = ResourceLoader.Load<PackedScene>(AsteroidScenePath);

		foreach (var enemy in Enemies)
		{
			_enemyScenes.Add(ResourceLoader.Load<PackedScene>(enemy));
		}
		
		SpawnAsteroids();

		SetupRound();

		_soundManager.Call("StartFlightNoise");
	}

	public override void _Process(double delta)
	{
		_progressBar.Value = (1 - ((float)_enemyCount / (float)_enemiesSpawned)) * 100.0f;
	}

	private void SetupRound()
	{
		PlayerVariables.Stats.Round++;

		_enemiesSpawned = 2 + PlayerVariables.Stats.DangerLevel;

		PlayerVariables.Stats.CurrentHealth = PlayerVariables.Stats.MaxHealth;
		PlayerVariables.Stats.CurrentShield = PlayerVariables.Stats.MaxShield;

		SpawnEnemyWave(_enemiesSpawned);
		
	}

	private void SpawnAsteroids()
	{
		if (_asteroidScene == null)
			return;

		for (int i = 0; i < AsteroidCount; i++)
		{
			Asteroid asteroid = _asteroidScene.Instantiate<Asteroid>();
			float angle = _rng.RandfRange(0.0f, Mathf.Pi * 2.0f);
			float radius = _rng.RandfRange(260.0f, 1200.0f);
			asteroid.Position = Vector2.Right.Rotated(angle) * radius;
			asteroid.AngularVelocity = _rng.Randf();
			asteroid.LinearVelocity = new Vector2(_rng.RandfRange(-10.0f, 10.0f), _rng.RandfRange(-10.0f, 10.0f));
			
			string material = _rng.Randi() % 2 == 0 ? "grey" : "gold";
			Sprite2D asteroidSprite = asteroid.GetNodeOrNull<Sprite2D>("Sprite2D");
			if (asteroidSprite != null)
			{
				asteroidSprite.Texture = ResourceLoader.Load<Texture2D>($"res://gfx/game/asteroids/{material}/asteroid_{material}_{_rng.RandiRange(1, 6)}.png");
			}

			float scaleFactor = _rng.RandfRange(0.5f, 2.5f);
			foreach (var child in asteroid.GetChildren().Cast<Node2D>())
			{
				child.Scale *= scaleFactor;
			}
			asteroid.Mass *= scaleFactor;
			
			PlayerVariables.Space.AddChild(asteroid);
		}
	}

	private void SpawnEnemyWave(int amount)
	{
		Node2D player = GetNodeOrNull<Node2D>("Player");
		Vector2 center = player?.GlobalPosition ?? Vector2.Zero;
		int danger = PlayerVariables.Stats.DangerLevel;

		for (int i = 0; i < amount; i++)
		{
			int index;
			// choose a random enemy from the loaded scenes
			do
			{
				index = _rng.RandiRange(0, _enemyScenes.Count - 1);
			}
			// reroll if the chosen enemy cannot spawn yet
			while (GetMinimumRound(_enemyScenes[index]) > PlayerVariables.Stats.Round);
			
			BaseEnemy enemy = _enemyScenes[index].Instantiate<BaseEnemy>();

			// subscribe to death signal
			enemy.Killed += () => _enemyCount--;

			float angle = (Mathf.Pi * 2.0f * i / amount) + _rng.RandfRange(-0.35f, 0.35f);
			float radius = _rng.RandfRange(420.0f, 650.0f + danger * 5.0f);
			enemy.Position = center + Vector2.Right.Rotated(angle) * radius;
			PlayerVariables.Space.AddChild(enemy);

			_enemyCount++;
		}
	}

	public async void OpenBuildMenuAfterDelay(float delay)
	{
		GD.Print("Changing scene...");
		_soundManager.Call("Menu");
		await ToSignal(GetTree().CreateTimer(delay), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile("res://scenes/player/BuildMenu.tscn");
	}

	/// <summary>
	/// Get the minimum round an enemy spawns in without instantiating it.
	/// It is stored in the respective scene file and can be edited in the Godot editor.
	/// </summary>
	private static int GetMinimumRound(PackedScene scene)
	{
		var state = scene.GetState();

		// unintuitively, you can only see properties which are overridden in a scene, not their default values
		int propCount = state.GetNodePropertyCount(0);

		// find the correct property (if it is amongst the overridden ones)
		for (int i = 0; i < propCount; i++)
		{
			if (state.GetNodePropertyName(0, i) == "MinimumRound")
				return (int)state.GetNodePropertyValue(0, i);
		}

		// otherwise it's zero
		return 0;
	}
}
