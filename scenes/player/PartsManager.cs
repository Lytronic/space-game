using Godot;
using System;
using System.Collections.Generic;


public partial class PartsManager : Node2D
{
	//player parent to set the script's Weapon reference to the selected launcher
	Player player;
	
	// these are the child nodes of PartsManager that will be referenced in the PLayer script in order to execute each Weapon's shooting method
	public BaseWeapon[] Weapons;

	public override void _Ready()
	{
		player = GetParent<Player>();

	}

	public void SwitchToWeapon(int weaponType)
	{
		player.Weapon = Weapons[weaponType];
	}
	private int addOrSubtractInt(bool add) // support method to streamline applying stats in subclasses
	{
		if (add)
		{
			return 1;
		}
		else { return -1; }
	}

}
