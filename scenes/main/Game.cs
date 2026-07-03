using Godot;

public partial class Game : Node2D
{
	private const string AsteroidScenePath = "res://scenes/enemies/AsteroidEnemy.tscn";
	private const string EnemyScenePath = "res://scenes/enemies/enemy bases/BaseEnemy.tscn";

	[Export] public int AsteroidCount = 55;

	private static readonly string[] EnemyTexturePaths =
	[
		"res://gfx/game/enemy/drone.png",
		"res://gfx/game/enemy/ship_ccc.png",
		"res://gfx/game/enemy/ship_eee.png",
		"res://gfx/game/enemy/ship_fff.png"
	];

	private PackedScene _asteroidScene;
	private PackedScene _enemyScene;
	private Texture2D[] _enemyTextures = [];

	private int _enemiesSpawned = 0;
	private int _enemyCount = 0;

	private RandomNumberGenerator _rng;

	private ProgressBar _progressbar;

	public override void _Ready()
	{
		_rng = new();
		_rng.Randomize();

		PlayerVariables.Instance.Space = GetNode<EntityManager>("EntityManager");

		_progressbar = GetNode<ProgressBar>("CanvasLayer/HUD/RoundIndicator/ProgressBar");

		_asteroidScene = ResourceLoader.Load<PackedScene>(AsteroidScenePath);
		_enemyScene = ResourceLoader.Load<PackedScene>(EnemyScenePath);
		_enemyTextures = LoadEnemyTextures();

		SpawnAsteroids();

		SetupRound();
	}

	public override void _Process(double delta)
	{
		_progressbar.Value = (1 - _enemyCount) / _enemiesSpawned;
	}

	private void SetupRound()
	{
		PlayerVariables.Instance.Round++;

		_enemiesSpawned = 2 + PlayerVariables.Instance.DangerLevel;
		SpawnEnemyWave(_enemiesSpawned);
		
	}

	private void SpawnAsteroids()
	{
		if (_asteroidScene == null)
			return;

		for (int i = 0; i < AsteroidCount; i++)
		{
			AsteroidEnemy asteroid = _asteroidScene.Instantiate<AsteroidEnemy>();
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

			asteroid.Scale *= _rng.RandfRange(0.5f, 1.5f);
			PlayerVariables.Instance.Space.AddChild(asteroid);
		}
	}

	private void SpawnEnemyWave(int amount)
	{
		if (_enemyScene == null)
			return;

		Node2D player = GetNodeOrNull<Node2D>("Player");
		Vector2 center = player?.GlobalPosition ?? Vector2.Zero;
		int danger = PlayerVariables.Instance.DangerLevel;

		for (int i = 0; i < amount; i++)
		{
			BaseEnemy enemy = _enemyScene.Instantiate<BaseEnemy>();
			ConfigureEnemy(enemy, i);

			float angle = (Mathf.Pi * 2.0f * i / amount) + _rng.RandfRange(-0.35f, 0.35f);
			float radius = _rng.RandfRange(420.0f, 650.0f + danger * 25.0f);
			enemy.Position = center + Vector2.Right.Rotated(angle) * radius;
			PlayerVariables.Instance.Space.AddChild(enemy);

			_enemiesSpawned++;
		}
	}

	private void ConfigureEnemy(BaseEnemy enemy, int index)
	{
		int archetype = index % 4;

		// BaseEnemy applies the danger multiplier when it enters the tree.
		enemy.Health = 28;
		enemy.Damage = 7.0f;
		enemy.Speed = 120.0f;
		enemy.Resistance = 0.08f;

		Sprite2D sprite = enemy.GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite != null && archetype < _enemyTextures.Length)
		{
			sprite.Texture = _enemyTextures[archetype];
			sprite.Scale = Vector2.One * (archetype == 0 ? 0.045f : 0.052f);
		}

		BaseEnemyAI ai = enemy.GetNodeOrNull<BaseEnemyAI>("BaseEnemyAi");
		if (ai != null)
		{
			ai.PreferredRange = _rng.RandfRange(230.0f, 310.0f);
			ai.RetreatRange = _rng.RandfRange(115.0f, 155.0f);
			ai.FireRange = 430.0f;
			ai.FireCooldown = _rng.RandfRange(1.0f, 1.7f);
			ai.ProjectileSpeed = _rng.RandfRange(300.0f, 370.0f);
			ai.AimSpreadRadians = _rng.RandfRange(0.04f, 0.12f);
		}
	}

	private static Texture2D[] LoadEnemyTextures()
	{
		Texture2D[] textures = new Texture2D[EnemyTexturePaths.Length];
		for (int i = 0; i < EnemyTexturePaths.Length; i++)
		{
			textures[i] = ResourceLoader.Load<Texture2D>(EnemyTexturePaths[i]);
		}

		return textures;
	}
	
	public async void OpenBuildMenuAfterDelay(float delay)
	{
		GD.Print("Changing scene...");
		await ToSignal(GetTree().CreateTimer(delay), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile("res://scenes/player/BuildMenu.tscn");
	}
}
