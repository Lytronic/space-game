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

	public int selectedAbilityNr;
	public ShipPart selectedAbilityName;

	public ShipPart[] Abilities; // Array for the order of abilities
	public TextureRect[] ChildSprites;


	public override void _Ready()
	{
		
		foreach (Node child in GetChildren()) // Hide the abilities without adding them to an array
		{
			if (child is Control control)
			{
				control.Hide();
			}
		}	
		Abilities = new ShipPart[8];
		ChildSprites = new TextureRect[8];
		for (int i = 0; i < ChildSprites.Length; i++)
		{
			ChildSprites[i] = GetChild<TextureRect>(i);
		}
		GD.Print(ChildSprites);
		UpdateAbilities();
		
	}

	
	public override void _Process(double delta)
	{
		
	}

	// Function to update the ability hud
	private void UpdateAbilities()
	{
		var _updater = 0;
		Array.Clear(Abilities, 0, Abilities.Length);
		if(true)
		{
			//ChildSprites[0].Show();
			ChildSprites[1].Show();
			//Abilities[_updater] = 
			_updater += 2;
		}
		else
		{
			ChildSprites[0].Hide();
			ChildSprites[1].Hide();
		}
		if(true)
		{
			ChildSprites[2].Show();
			//Abilities[_updater] = 
			_updater += 1;
		}
		else
		{
			ChildSprites[2].Hide();
		}
	}
	private void ChangeAbility(int AbilityNr)
	{
		if (Abilities[AbilityNr] != null)
		{
			selectedAbilityNr = AbilityNr;
			selectedAbilityName = Abilities[AbilityNr];
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (keyEvent.Keycode == Key.Q)
			{
				ChangeAbility(0);
			}
			if (keyEvent.Keycode == Key.Key1)
			{
				ChangeAbility(1);
			}
			if (keyEvent.Keycode == Key.Key2)
			{
				ChangeAbility(2);
			}
			if (keyEvent.Keycode == Key.Key3)
			{
				ChangeAbility(3);
			}
			if (keyEvent.Keycode == Key.Key4)
			{
				ChangeAbility(4);
			}
			if (keyEvent.Keycode == Key.Key5)
			{
				ChangeAbility(5);
			}
			if (keyEvent.Keycode == Key.Key6)
			{
				ChangeAbility(6);
			}
			if (keyEvent.Keycode == Key.Key7)
			{
				ChangeAbility(7);
			}
		}
	}

	
}
