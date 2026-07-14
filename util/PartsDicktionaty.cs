using Godot;
using System;
using System.Collections.Generic;

public partial class PartsDicktionaty : Node
{
    public static PartsDicktionaty Instance { get; private set; }


    public override void _Ready()
    {
        Instance = this;
    }


    public static readonly Dictionary<string, ShipPart> ReconstructParts = new()
    {
        ["DebugMultitool"] = new DebugMultitool(),
        ["WeaponPartArc"] = new WeaponPartArc(),
        ["WeaponPartCannon"] = new WeaponPartCannon(),
        ["WeaponCoil"] = new WeaponCoil(),
        ["WeaponPartEmp"] = new WeaponPartEmp(),
        ["WeaponPartLaser"] = new WeaponPartLaser(),
        ["WeaponPartMissile"] = new WeaponPartMissile(),
        ["WeaponPartPlasma"] = new WeaponPartPlasma(),
        ["WeaponPartRail"] = new WeaponPartRail(),
        ["WeaponPartTorpedo"] = new WeaponPartTorpedo()


    };

}

