using Godot;
using System;

public partial class ShootingGroundForEnemies : Node2D
{
	public async void OpenBuildMenuAfterDelay(float delay)
	{
		GD.Print("Changing scene...");
		await ToSignal(GetTree().CreateTimer(delay), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile("res://scenes/player/BuildMenu.tscn");
	}
}
