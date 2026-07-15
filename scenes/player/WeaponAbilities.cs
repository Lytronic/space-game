using Godot;
using System;
using System.ComponentModel;

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
	public BaseWeapon selectedAbilityName;

	public int[] Abilities; // Array for the order of abilities
	public TextureRect[] ChildSprites;

	private Player _player;
	private PartsManager _partsManager;
	private WeaponsControls _weaponsControls;
	

	public override void _Ready()
	{
		_weaponsControls = GetNode<WeaponsControls>("/root/game/CanvasLayer/HUD/Weapons/WeaponsControls");
		_player = GetNode<Player>("/root/game/Player");
		_partsManager = GetNode<PartsManager>("/root/game/Player/PartsManager");
		foreach (Node child in GetChildren()) // Hide the abilities without adding them to an array
		{
			if (child is Control control)
			{
				//control.Hide();
			}
		}	
		Abilities = new int[7];
		ChildSprites = new TextureRect[7];
		for (int i = 0; i < ChildSprites.Length; i++)
		{
			ChildSprites[i] = GetChild<TextureRect>(i);
		}
		//_weaponsControls.updateControls();
		ChangeAbility(0);
	}

	
	public override void _Process(double delta)
	{
		
	}

	// Function to update the ability hud
	public void UpdateAbilities()
	{

		ChildSprites[0].Texture = TexturePlasmaOff;

	}
	public void ChangeAbility(int AbilityNr)
	{
		if(PlayerVariables.Instance.WeaponList[AbilityNr] != 0)
		{
		switch (AbilityNr)
		{
			case 0:
				ChildSprites[0].Texture = TexturePlasma;
				break;
			case 1:
				ChildSprites[0].Texture = TextureArc;
				break;
			case 2:
				ChildSprites[0].Texture = TextureSlug;
				break;
			case 3:
				ChildSprites[0].Texture = TextureEMP;
				break;
			case 4:
				ChildSprites[0].Texture = TextureLaser;
				break;
			case 5:
				ChildSprites[0].Texture = TextureMissile;
				break;
			case 6:
				ChildSprites[0].Texture = TextureTorpedo;
				break;
		}
		}
		else
			{
				switch (AbilityNr)
		{
			case 0:
				ChildSprites[0].Texture = TexturePlasmaOff;
				break;
			case 1:
				ChildSprites[0].Texture = TextureArcOff;
				break;
			case 2:
				ChildSprites[0].Texture = TextureSlugOff;
				break;
			case 3:
				ChildSprites[0].Texture = TextureEMPOff;
				break;
			case 4:
				ChildSprites[0].Texture = TextureLaserOff;
				break;
			case 5:
				ChildSprites[0].Texture = TextureMissileOff;
				break;
			case 6:
				ChildSprites[0].Texture = TextureTorpedoOff;
				break;
		}
			}

		
		
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (keyEvent.Keycode == Key.Key1)
			{
				ChangeAbility(0);
			}
			if (keyEvent.Keycode == Key.Key2)
			{
				ChangeAbility(1);
			}
			if (keyEvent.Keycode == Key.Key3)
			{
				ChangeAbility(2);
			}
			if (keyEvent.Keycode == Key.Key4)
			{
				ChangeAbility(3);
			}
			if (keyEvent.Keycode == Key.Key5)
			{
				ChangeAbility(4);
			}
			if (keyEvent.Keycode == Key.Key6)
			{
				ChangeAbility(5);
			}
			if (keyEvent.Keycode == Key.Key7)
			{
				ChangeAbility(6);
			}
		}
	}

	
}
