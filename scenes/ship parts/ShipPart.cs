using Godot;
using System;

[GlobalClass]
public partial class ShipPart : Resource
{
    //these are the non-optional stats for each item
    public int danger;
    [Export] public virtual string displayTooltip { get; set; }
    [Export] public virtual float rarity { get; set; }
    [Export] public virtual bool isActive { get; set; }
    [Export] public virtual float partWeight { get; set; }

    [Export] public virtual Texture2D SpriteTexture { get; set; }

    public void Initialize()
    {
        danger = PlayerVariables.Stats.DangerLevel;
    }
    public virtual void activateEffect() { }

    public virtual void changeStats(bool add) //changes the player's stats: if add = false --> player stats are decreased | if add = true --> player stats are increased
    {
        //int addOrSubtract = add ? 1 : -1;

        //PlayerVariables.Instance.playerStat += (partStat * addOrSubtract)  <--- this is very important information but just a schema to show how to do it
    }
    public int addOrSubtractInt(bool b) // support method to streamline applying stats in subclasses
    {
        return b ? 1 : -1;
    }
    public float statRandomness() //suppoer method to streamline scaling in subclasses
    {
        float dangerMultiplier = GD.RandRange(1, danger);
        rarity *= dangerMultiplier;
        return dangerMultiplier;
    }
    public virtual void generateStats()
    {
        float mult = statRandomness();
    }
}
