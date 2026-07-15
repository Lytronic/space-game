using Godot;
using System;
using System.Collections.Generic;


public partial class PartsManager : Node2D
{
	//player parent to set the script's Weapon reference to the selected launcher
	public Player player;
	
	// these are the child nodes of PartsManager that will be referenced in the PLayer script in order to execute each Weapon's shooting method
	public BaseWeapon[] Weapons;

	//weapons that are children 
	public PackedScene Plasma = ResourceLoader.Load<PackedScene>("res://scenes/weapons/scenes/PlasmaGun.tscn");
    public PackedScene Arc = ResourceLoader.Load<PackedScene>("res://scenes/weapons/scenes/Arc.tscn");
    public PackedScene Cannon = ResourceLoader.Load<PackedScene>("res://scenes/weapons/scenes/CannonGun.tscn");
    public PackedScene Emp = ResourceLoader.Load<PackedScene>("res://scenes/weapons/scenes/EMP.tscn");
    public PackedScene Laser = ResourceLoader.Load<PackedScene>("res://scenes/weapons/scenes/Laser.tscn");
    public PackedScene Missile = ResourceLoader.Load<PackedScene>("res://scenes/weapons/scenes/MissileGun.tscn");
    public PackedScene Torpedo = ResourceLoader.Load<PackedScene>("res://scenes/weapons/scenes/TorpedoGun.tscn"); 
	public override void _Ready()
	{
		player = GetParent<Player>();
		makeChildren();

    }
	

	public void SwitchToWeapon(int weaponType)
	{
        GD.Print($"switched weapoon to: {weaponType} " + Weapons[weaponType]);
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
	//instantiates all the weapons
	public void makeChildren()
	{
		Weapons = new BaseWeapon[]
		{
			Plasma.Instantiate<PlasmaGun>(),
			Arc.Instantiate<RaycastWeapon>(),
			Cannon.Instantiate<CannonGun>(),
			Emp.Instantiate<EMP>(),
			Laser.Instantiate<RaycastWeapon>(),
			Missile.Instantiate<MissileGun>(),
			Torpedo.Instantiate<TorpedoGun>(),
		};

		foreach (var weapon in Weapons)
		{
			AddChild(weapon);
		}
		
    }
}
