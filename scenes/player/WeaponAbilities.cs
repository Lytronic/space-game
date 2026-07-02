using Godot;
using System;

public partial class WeaponAbilities : HBoxContainer
{
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
	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is Control control)
			{
				control.Hide();
			}
		}	
		Abilities = new TextureRect[7];
		UpdateAbilities();
	}

	
	public override void _Process(double delta)
	{
		
	}

	private void UpdateAbilities()
	{
		for (int i = 0; i < Abilities.Length; i++)
		{
			
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
