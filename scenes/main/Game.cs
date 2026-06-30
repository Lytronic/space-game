using Godot;
using System;

public partial class Game : Node2D
{
	public override void _Ready()
	{
		RandomNumberGenerator rng = new();
		
		for (int i = 0; i <= 100; i++)
		{
			AsteroidEnemy asteroid = (AsteroidEnemy)ResourceLoader.Load<PackedScene>("res://scenes/enemies/AsteroidEnemy.tscn").Instantiate();
			asteroid.Position = new Vector2(rng.RandfRange(-500.0f, 500.0f), rng.RandfRange(-500.0f, 500.0f));
			asteroid.AngularVelocity = rng.Randf();
			asteroid.LinearVelocity = new Vector2(rng.RandfRange(-10.0f, 10.0f), rng.RandfRange(-10.0f, 10.0f));
			
			string material = rng.Randi() % 2 == 0 ? "grey" : "gold";
			asteroid.GetChild<Sprite2D>(0).Texture = ResourceLoader.Load<Texture2D>($"res://gfx/game/asteroids/{material}/asteroid_{material}_{rng.RandiRange(1, 6)}.png");
			asteroid.Scale *= rng.RandfRange(0.5f, 1.5f);
			AddChild(asteroid);
		}
	}
	
	public async void OpenBuildMenuAfterDelay(float delay)
	{
		GD.Print("Changing scene...");
		await ToSignal(GetTree().CreateTimer(delay), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile("res://scenes/player/BuildMenu.tscn");
	}
}
