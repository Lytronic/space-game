using Godot;
using System;

public partial class WeaponCoil : ShipPart
{
    //non-optional stats
    [Export] public override string displayTooltip { get; set; } = "Mayday! Mayday! ... is this fluffin' thing even working?!";
    [Export] public override float rarity { get; set; } = 0.1f;
    [Export] public override bool isActive { get; set; } = false;
}
