using System;
using System.Collections;
using Godot;

public partial class Mirror : ShipPart
{
    [Export] public override string displayTooltip { get; set; } = "Mirror incoming laser beams";
    [Export] public override float rarity { get; set; } = 0.5f;
    [Export] public override bool isActive { get; set; } = false;
    [Export] public override string type { get; set; } = "Mirror";
    [Export] public override float partWeight { get; set; } = 9.11f;
    [Export] public override Texture2D SpriteTexture { get; set; }
    [Export] public override Texture2D MenuTexture { get; set; }

    [Export] public override string SpriteTexturePath { get; set; } = "res://gfx/gui/construction/icon/armor/armor_mirror.png";
    [Export] public override string MenuTexturePath { get; set; } = "res://gfx/gui/construction/icon/armor/armor_mirror.png";

	public override BitArray Shape { get; set; } = new(new bool[]{
		true, true, false,
		true, true, false,
		false, false, false
	});
}
