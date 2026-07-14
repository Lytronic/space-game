using Godot;
using System;
using System.Collections.Generic;


public partial class PartsManager : Node2D
{
    //player parent to set the script's Weapon reference to the selected launcher
    Player player;

    //weapons loaded
    private PackedScene _projectileLauncher;
    private PackedScene _LauncherArc;
    private PackedScene _LauncherCannon;
    private PackedScene _LauncherCoil;
    private PackedScene _LauncherEMP;
    private PackedScene _LauncherLaser;
    private PackedScene _LauncherMissile;
    private PackedScene _LauncherPlasma;
    private PackedScene _LauncherRail;
    private PackedScene _LauncherTorpedo;

    private string _projectileLauncherPath = "";
    private string _ArcPath = "res://scenes/ship parts/weapons/scripts/LauncherArc.cs";
    private string _CannonPath = "res://scenes/ship parts/weapons/scripts/LauncherCannon.cs";
    private string _CoilPath = "res://scenes/ship parts/weapons/scripts/LauncherCoil.cs";
    private string _EMPPath = "res://scenes/ship parts/weapons/scripts/LauncherEmp.cs";
    private string _LaserPath = "res://scenes/ship parts/weapons/scripts/LauncherLaser.cs";
    private string _MissilePath = "res://scenes/ship parts/weapons/scripts/LauncherMissile.cs";
    private string _PlasmaPath = "res://scenes/ship parts/weapons/scripts/LauncherPlasma.cs";
    private string _RailPath = "res://scenes/ship parts/weapons/scripts/LauncherRail.cs";
    private string _TorpedoPath = "res://scenes/ship parts/weapons/scripts/LauncherTorpedo.cs";

    // each integer represents the amopunt of weapons equipped for that type of weapon, this will be changed by the equipped ShipParts of type Weapon
    public int[] WeaponList = new int[10] 
    {   
        0,  // this index (0) represents the amount of start weapon the player has equipped
        0,  // Arc
        0,  // Cannon
        0,  // Coil
        0,  // EMP
        0,  // Laser
        0,  // Missile
        0,  // Plasma
        0,  // Rail
        0   // Torpedo
    };

    // these are the child nodes of PartsManager that will be referenced in the PLayer script in order to execute each Weapon's shooting method
    public BaseWeapon[] Launchers = new BaseWeapon[10]
    {
        new ProjectileLauncher(),
        new LauncherArc(),
        new LauncherCannon(),
        new LauncherCoil(),
        new LauncherEmp(),
        new LauncherLaser(),
        new LauncherMissile(),
        new LauncherPlasma(),
        new LauncherRail(),
        new LauncherTorpedo()

    };
    public override void _Ready()
    {
        player = GetParent<Player>();

        //spawn all the projectile Launchers in the scene
        //_projectileScene = ResourceLoader.Load<PackedScene>(ProjectileScene);
        _projectileLauncher = ResourceLoader.Load<PackedScene>(_projectileLauncherPath);
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

    //increases the weapon count for a weapon type if true ----> decreases if false
    //the index indicates the type of weapon which's amount 's being changed (the index of the weapon type should be the same for each list)
    public void AddWeapon(int index , bool addOrSubtract) 
    {

        WeaponList[index] += addOrSubtractInt(addOrSubtract);
    }
}
