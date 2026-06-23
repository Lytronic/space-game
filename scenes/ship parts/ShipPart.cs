using Godot;
using System;

[GlobalClass]
public partial class ShipPart : Node2D
{
    public string displayTooltip;
    public float rarity;
    public bool isActive;



    public void activateEffect()
    {

    }

    public void generateStats()
    {
        displayTooltip = "the tooltip";
        //randomise 
        rarity = 0.3f;
    }
}
