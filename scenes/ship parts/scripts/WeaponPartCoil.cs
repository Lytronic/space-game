using Godot;
using System;

public partial class WeaponCoil : ShipPart
{
    //non-optional stats
    [Export] public override string displayTooltip { get; set; } = "Mayday! Mayday! ... is this fluffin' thing even working?!";
    [Export] public override float rarity { get; set; } = 0.1f;
    [Export] public override bool isActive { get; set; } = false;
    [Export] public override string type { get; set; } = "WeaponCoil";
    [Export] public override float partWeight { get; set; } = 9.11f;
    [Export] public override Texture2D SpriteTexture { get; set; }
    [Export] public override Texture2D MenuTexture { get; set; }

    [Export] public override string SpriteTexturePath { get; set; } = "res://gfx/gui/parts/weapon_coil.png";
    [Export] public override string MenuTexturePath { get; set; } = "res://gfx/gui/construction/icon/weaponry/weapon_coil.png";

    public override void changeStats(bool add)
    {

    }
}
