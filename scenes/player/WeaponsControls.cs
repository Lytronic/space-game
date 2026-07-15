using Godot;
using System;

public partial class WeaponsControls : HBoxContainer
{
	Label[] ControlKeys; // Array for the keyboard key labels
	private WeaponAbilities _weaponAbilities; // Reference for the WeaponAbilities

	public override void _Ready()
	{
		_weaponAbilities = GetNode<WeaponAbilities>("/root/game/CanvasLayer/HUD/Weapons/WeaponAbilities");
		ControlKeys = new Label[8]; // Initialize ControlKeys Array
		ControlKeys[0] = GetChild(0) as Label; // Add the Q key label to the array
		for (int i = 1; i < ControlKeys.Length; i++)
		{
			ControlKeys[i] = GetChild(i) as Label; // Add the rest to the array
		}
		for (int i = 0; i < ControlKeys.Length; i++)
		{
			ControlKeys[i].Hide();
		}
		//updateControls();
	}


	public override void _Process(double delta)
	{
	}
	
	public void updateControls() {
		GD.Print(_weaponAbilities.Abilities);
		for (int i = 0; i < ControlKeys.Length; i++)
		{
			if(_weaponAbilities.Abilities[i] != null)
			{
				ControlKeys[i].Show();
			}
			else
			{
				ControlKeys[i].Hide();
			}
		}
	}
}
