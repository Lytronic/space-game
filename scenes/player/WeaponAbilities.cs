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

	public BaseWeapon[] Abilities; // Array for the order of abilities
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
				control.Hide();
			}
		}	
		Abilities = new BaseWeapon[8];
		ChildSprites = new TextureRect[8];
		for (int i = 0; i < ChildSprites.Length; i++)
		{
			ChildSprites[i] = GetChild<TextureRect>(i);
		}
		UpdateAbilities();
		_weaponsControls.updateControls();
	}

	
	public override void _Process(double delta)
	{
		
	}

	// Function to update the ability hud
	public void UpdateAbilities()
	{
		var _updater = 1;
		Array.Clear(Abilities, 0, Abilities.Length);
		if(PlayerVariables.Instance.WeaponList[0] != 0)
		{
			//ChildSprites[0].Show();
			ChildSprites[1].Show();
			Abilities[_updater] = _partsManager.Weapons[0];
			_updater += 1;
		}
		else
		{
			ChildSprites[0].Hide();
			ChildSprites[1].Hide();
		}
		if(PlayerVariables.Instance.WeaponList[1] != 0)
		{
			ChildSprites[2].Show();
			Abilities[_updater] = _partsManager.Weapons[1];
			_updater += 1;
		}
		else
		{
			ChildSprites[2].Hide();
		}
		if(PlayerVariables.Instance.WeaponList[2] != 0)
		{
			ChildSprites[3].Show();
			Abilities[_updater] = _partsManager.Weapons[2];
			_updater += 1;
		}
		else
		{
			ChildSprites[3].Hide();
		}
		if(PlayerVariables.Instance.WeaponList[3] != 0)
		{
			ChildSprites[4].Show();
			Abilities[_updater] = _partsManager.Weapons[3];
			_updater += 1;
		}
		else
		{
			ChildSprites[4].Hide();
		}
		if(PlayerVariables.Instance.WeaponList[4] != 0)
		{
			ChildSprites[5].Show();
			Abilities[_updater] = _partsManager.Weapons[4];
			_updater += 1;
		}
		else
		{
			ChildSprites[5].Hide();
		}
		if(PlayerVariables.Instance.WeaponList[5] != 0)
		{
			ChildSprites[6].Show();
			Abilities[_updater] = _partsManager.Weapons[5];
			_updater += 1;
		}
		else
		{
			ChildSprites[6].Hide();
		}
		if(PlayerVariables.Instance.WeaponList[6] != 0)
		{
			ChildSprites[7].Show();
			Abilities[_updater] = _partsManager.Weapons[6];
			_updater += 1;
		}
		else
		{
			ChildSprites[7].Hide();
		}

		ChildSprites[0].Texture = TextureSwapOff;
		
		ChildSprites[3].Texture = TextureSlugOff;

		ChildSprites[6].Texture = TextureMissileOff;

		ChildSprites[7].Texture = TextureTorpedoOff;
			
		ChildSprites[1].Texture = TexturePlasmaOff;
		
		ChildSprites[5].Texture = TextureLaserOff;
			
		ChildSprites[2].Texture = TextureArcOff;
			
		ChildSprites[4].Texture = TextureEMPOff;


	}
	public void ChangeAbility(int AbilityNr)
	{
		GD.Print("Changing Ability!" + AbilityNr);
		if (Abilities[AbilityNr] != null)
		{
			selectedAbilityNr = AbilityNr;
			selectedAbilityName = Abilities[AbilityNr];
			for (int i = 0; i < ChildSprites.Length; i++)
			{
				if(i == AbilityNr)
				{
					if(ChildSprites[i].Texture == TexturePlasma || ChildSprites[i].Texture == TexturePlasmaOff)
					{
						ChildSprites[i].Texture = TexturePlasma;
					}
					if(ChildSprites[i].Texture == TextureArc || ChildSprites[i].Texture == TextureArcOff)
					{
						ChildSprites[i].Texture = TextureArc;
					}
					if(ChildSprites[i].Texture == TextureSwap || ChildSprites[i].Texture == TextureSwapOff)
					{
						ChildSprites[i].Texture = TextureSwap;
					}
					if(ChildSprites[i].Texture == TextureSlug || ChildSprites[i].Texture == TextureSlugOff)
					{
						ChildSprites[i].Texture = TextureSlug;
					}
					if(ChildSprites[i].Texture == TextureMissile || ChildSprites[i].Texture == TextureMissileOff)
					{
						ChildSprites[i].Texture = TextureMissile;
					}
					if(ChildSprites[i].Texture == TextureTorpedo || ChildSprites[i].Texture == TextureTorpedoOff)
					{
						ChildSprites[i].Texture = TextureTorpedo;
					}
					if(ChildSprites[i].Texture == TextureLaser || ChildSprites[i].Texture == TextureLaserOff)
					{
						ChildSprites[i].Texture = TextureLaser;
					}
					if(ChildSprites[i].Texture == TextureEMP || ChildSprites[i].Texture == TextureEMPOff)
					{
						ChildSprites[i].Texture = TextureEMP;
					}
				}
				else
				{
					if(ChildSprites[i].Texture == TexturePlasma || ChildSprites[i].Texture == TexturePlasmaOff)
					{
						ChildSprites[i].Texture = TexturePlasmaOff;
					}
					if(ChildSprites[i].Texture == TextureArc || ChildSprites[i].Texture == TextureArcOff)
					{
						ChildSprites[i].Texture = TextureArcOff;
					}
					if(ChildSprites[i].Texture == TextureSwap || ChildSprites[i].Texture == TextureSwapOff)
					{
						ChildSprites[i].Texture = TextureSwapOff;
					}
					if(ChildSprites[i].Texture == TextureSlug || ChildSprites[i].Texture == TextureSlugOff)
					{
						ChildSprites[i].Texture = TextureSlugOff;
					}
					if(ChildSprites[i].Texture == TextureMissile || ChildSprites[i].Texture == TextureMissileOff)
					{
						ChildSprites[i].Texture = TextureMissileOff;
					}
					if(ChildSprites[i].Texture == TextureTorpedo || ChildSprites[i].Texture == TextureTorpedoOff)
					{
						ChildSprites[i].Texture = TextureTorpedoOff;
					}
					if(ChildSprites[i].Texture == TextureLaser || ChildSprites[i].Texture == TextureLaserOff)
					{
						ChildSprites[i].Texture = TextureLaserOff;
					}
					if(ChildSprites[i].Texture == TextureEMP || ChildSprites[i].Texture == TextureEMPOff)
					{
						ChildSprites[i].Texture = TextureEMPOff;
				}			
			}
		}
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
