using Godot;
using System;

[GlobalClass]
public partial class ShipPart : Node2D
{
    //these are the non-optional stats for each item
    public int danger;
    [Export] public string displayTooltip;
    [Export] public float rarity;
    [Export] public bool isActive;
    [Export] public float partWeight;

    public override void _EnterTree()
    {
    danger = PlayerVariables.Instance.danger_level;
    }
    public virtual void activateEffect() { }

    public virtual void changeStats(bool add) //changes the player's stats: if add = false --> player stats are decreased | if add = true --> player stats are increased
    {
        int addOrSubtract = 0;
        if(add) {addOrSubtract = 1;}
        else {addOrSubtract = -1;}

        //PlayerVariables.Instance.playerStat += (partStat * addOrSubtract)  <--- this is very important information but just a schema to show how to do it
    }
    public int addOrSubtractInt(bool b) // support method to streamline applying stats in subclasses
    {
        if (b) { return 1; }
        else { return -1; }
    }
    public float statRandomness() //suppoer method to streamline scaling in subclasses
    {
        float dangerMultiplier = GD.RandRange(1, danger);
        rarity = rarity * dangerMultiplier;
        return dangerMultiplier;
    }
    public virtual void generateStats()
    {
        float mult = statRandomness();
    }
}
