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

    //variables needed to recreate the item from save
    [Export] public virtual string itemType { get; set; }
    [Export] public virtual float randomness { get; set; } = -1f;

    [Export] public virtual Texture2D SpriteTexture { get; set; }

    /*
     * Jedes part hat eine Form.
     * Diese wird durch ein zweidimensionales boolean array beschrieben.
     * 'true' = besetzt
     * 'false' = unbesetzt
     * Die größte Form ist 3x3 (alles 'true')
     * Die kleinste Form ist 1x1 (siehe unten)
     */
    public bool[,] Shape = {
    { false, false, false },
    { false, true, false },
    { false, false, false }
    };

    public void Initialize()
    {
        danger = PlayerVariables.Stats.DangerLevel;
        
    }
    public virtual void activateEffect() { }

    public virtual void changeStats(bool add) //changes the player's stats: if add = false --> player stats are decreased | if add = true --> player stats are increased
    {
        //int addOrSubtract = add ? 1 : -1;

        //PlayerVariables.Instance.playerStat += (partStat * addOrSubtract)  <--- this is very important information but just a schema to show how to do it
        
        //here the weapon adds a child to the partsManager for actual shooting
    }
    public int addOrSubtractInt(bool b) // support method to streamline applying stats in subclasses
    {
        return b ? 1 : -1;
    }
    public void GenerateStatRandomness() //support method to streamline scaling in subclasses
    {
        float dangerMultiplier;

        if(randomness < 0f)
        {
            dangerMultiplier = GD.RandRange(1, danger);
            rarity *= dangerMultiplier;
            randomness = dangerMultiplier;
        }
        else
        {
            rarity *= randomness;
        }
        
    }
    public virtual void generateStats()
    {
        GenerateStatRandomness();
        //randomness is then applied to all stats
    }

    public (string type, float reandomness) GetDefiningValues()
    {
        return (itemType , randomness);
    }

}
