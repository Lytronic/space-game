using Godot;
using System;
using System.Collections.Generic;


public partial class PartsManager : Node2D
{
    //player parent to set the script's Weapon reference to the selected launcher
    Player player;

    // each integer represents the amopunt of weapons equipped for that type of weapon, this will be changed by the equipped ShipParts of type Weapon
    public int[] WeaponList = new int[7] 
    {   
        0,  // this index (0) represents the amount of start weapon the player has equipped
        0,  //
        0,  //
        0,  //
        0,  //
        0,  //
        0   //
    };

    // these are the child nodes of PartsManager that will be referenced in the PLayer script in order to execute each Weapon's shooting method
    public BaseWeapon[] Launchers = new BaseWeapon[7]
    {
        new ProjectileLauncher(),
        new BaseWeapon(),
        new BaseWeapon(),
        new BaseWeapon(),
        new BaseWeapon(),
        new BaseWeapon(),
        new BaseWeapon()

    };
    public override void _Ready()
    {
        player = GetParent<Player>();
    }
    public void SwitchToWeapon(int weaponType)
    {
        player.Weapon = Launchers[weaponType];
    }
    private int addOrSubtractInt(bool add) // support method to streamline applying stats in subclasses
    {
        if (add)
        {
            return 1;
        }
        else { return -1; }
    }

    public void AddWeapon(int index , bool addOrSubtract) //increases the weapon count for a weapon type if true ----> decreases if false
    {

        WeaponList[index] += addOrSubtractInt(addOrSubtract);
    }
}
