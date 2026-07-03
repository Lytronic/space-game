using Godot;
using System;

public partial class WeaponAbilities : HBoxContainer
{
	// Export variables for the Ability Textures so that they can be called more easily
	[Export] public Texture2D TextureSwap;
	[Export] public Texture2D TextureSwapOff;
	[Export] public Texture2D TextureSlug;
	[Export] public Texture2D TextureSlugOff;
	[Export] public Texture2D TextureMissile;
	[Export] public Texture2D TextureMissileOff;
	[Export] public Texture2D TextureTorpedo;
	[Export] public Texture2D TextureTorpedoOff;
	[Export] public Texture2D TexturePlasma;
	[Export] public Texture2D TexturePlasmaOff;
	[Export] public Texture2D TextureLaser;
	[Export] public Texture2D TextureLaserOff;
	[Export] public Texture2D TextureArc;
	[Export] public Texture2D TextureArcOff;
	[Export] public Texture2D TextureEMP;
	[Export] public Texture2D TextureEMPOff;

	TextureRect[] Abilities; // Array for the order of abilities

	Label[] ControlKeys; // Array for the keyboard key label paths

	public override void _Ready()
	{
		ControlKeys = new Label[8]; // Initialize ControlKeys Array
		ControlKeys[0] = GetNode<Label>("/root/Control/WeaponsControls/ControlQ"); // Add the Q key label to the array
		for (int i = 1; i < ControlKeys.Length; i++)
		{
			ControlKeys[i] = GetNode<Label>($"/root/Control/WeaponsControls/Control{i}"); // Add the rest to the array
		}
		foreach (Node child in GetChildren()) // Hide the abilities without adding them to an array
		{
			if (child is Control control)
			{
				control.Hide();
			}
		}	
		Abilities = new TextureRect[8];
		UpdateAbilities();
	}

	
	public override void _Process(double delta)
	{
		
	}

	// Function to update the ability hud
	private void UpdateAbilities()
	{
		for (int i = 0; i < Abilities.Length; i++)
		{
			
		}
		for (int i = 0; i < ControlKeys.Length; i++)
		{
			if(Abilities[i] != null)
			{
				ControlKeys[i].Show();
			}
			else
			{
				ControlKeys[i].Hide();
			}
		}
	}
	private void UseAbility(int AbilityNr)
	{
		if (Abilities[AbilityNr] != null)
		{
			
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (keyEvent.Keycode == Key.Q)
			{
				UseAbility(0);
			}
			if (keyEvent.Keycode == Key.Key1)
			{
				UseAbility(1);
			}
			if (keyEvent.Keycode == Key.Key2)
			{
				UseAbility(2);
			}
			if (keyEvent.Keycode == Key.Key3)
			{
				UseAbility(3);
			}
			if (keyEvent.Keycode == Key.Key4)
			{
				UseAbility(4);
			}
			if (keyEvent.Keycode == Key.Key5)
			{
				UseAbility(5);
			}
			if (keyEvent.Keycode == Key.Key6)
			{
				UseAbility(6);
			}
			if (keyEvent.Keycode == Key.Key7)
			{
				UseAbility(7);
			}
		}
	}

	
}
