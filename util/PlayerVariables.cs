using System.Collections.Generic;
using Godot;

public partial class PlayerVariables : Node
{
    public static PlayerVariables Instance { get; private set; }
    public Node Space { get; set; }

    // The player's current score
    public int Score = 0;

    // difficulty level scales all enemy power exponentially
    public int DangerLevel {  get; private set; } = 1;
    public float LuckStat { get; set; } = 1;

    //durrability stats stuff: armor and shield toughness are scaling stats reducing percentual damage scaling in a power curve | Math.Pow
    public float MaxHealth { get; set; } = 100.0f;
    public float CurrentHealth { get; set; } = 100.0f;
    public float MaxShield { get; set; } = 100.0f;
    public float CurrentShield { get; set; } = 0.0f;
    public float ArmorToughness {  get; set; } = 1;
    public float ShieldToughness { get; set; } = 1;
    public int ShieldRegen {  get; set; } = 1; // regen is a flat increase in current shield that gets applied AT THE END OF EVERY FULL SECOND 

    //ship resources 
    public int Ammo { get; set; } = 1;
    public float Energy { get; set; } = 1;
    public float MaxEnergy { get; set; } = 1;
    public float Fuel { get; set; } = 1;
    public float MaxFuel { get; set; } = 1;
    public float EnergyGeneration {  get; set; } = 1;

    //this should set max speed and acceleration (thrust against weight)
    public float Thrust { get; set; } = 1; //max speed and (acceleration hindered by weight)
    public float Weight { get; set; } = 1;
    public float Control { get; set; } = 1;// how much the weight influences the acceleration, deceleraton and steering (0.0f - 1.0f)

    //damage will be calculated through percentual damage increase and flat damage multiplication (modifier are being multiplied, mod > 1 --> increase; mod < 1 --> decrease)
    public float DamageModif {  get; set; } = 1;
    public float DamgeBase { get; set; } = 1; //universial damage buff flat 
    public float PhysDamage { get; set; } = 1;
    public float PhysicalDmgMod {  get; set; } = 1; // percentage increased physical damage
    public float EnergyDamage { get; set; } = 1;
    public float EnergyDmgMod { get; set; } = 1; // percentage increased energy damage 

    //this will store all the items the player has in their inventory
    public List<ShipPart> PlayerActiveParts { get; private set; } = []; // active parts that get activate an effect every time their cooldown is down or under a condition
    public List<ShipPart> PlayerPassiveParts { get; private set; } = []; // passive parts that only apply an effect on the time they are added to the ship
    public List<ShipPart> PlayerCollectedParts { get; set; } = []; // basically the stash that the game uses to store all the loot at the end of a round (this gets reset every new round)




    public override void _Ready()
    {
        Instance = this;
    }

    //changing difficulty or setting difficult easily (for settings and items)
    public void ChangeDifficulty(int change, bool setToValue)
    {
        if (setToValue)
        {
            DangerLevel = change;
        }
        else
        {
            DangerLevel += change;
        }
    }

    //overload to more easily increase difficulty
    public void ChangeDifficulty(int change)
    {
        DangerLevel += change;
    }

    // --------------------- managing the ship parts attatched and not attatched and activae and passive ---------------------

    //adding a part to the ship
    public void AddPartToShip(ShipPart part)    
    {
        if(part.isActive)
        {
            PlayerActiveParts.Add(part);
        }
        if(!part.isActive)
        {
            PlayerPassiveParts.Add(part);
            part.changeStats(true); //adds the part's stats to the player's stats
        }
        PlayerCollectedParts.Remove(part);
        //GD.Print($"Part moved!! moved {part} from collection to ship");//debug
    }

    public void RemovePartFromShip(ShipPart part)
    {
        if (part.isActive)
        {
            PlayerActiveParts.Remove(part);
        }
        if (!part.isActive)
        {
            PlayerPassiveParts.Remove(part);
            part.changeStats(false); //subtracts the part's stats from the player stats
        }
        PlayerCollectedParts.Add(part);
        //GD.Print($"Part moved!! moved {part} from ship to collection "); //debug
    }

    public void AddLootToCollection(ShipPart[] array)
    {
        PlayerCollectedParts.AddRange(array);
        GD.Print($"added parts from loot {array} to collection");
        CheckForDebugItem(array);
        array = null;
    }

    //if there's a DebugMultitool then I want it to test the other methods too just because I'm not making another testing scenario ^~^
    private void CheckForDebugItem(ShipPart[] parts)
    {
        foreach(ShipPart part in parts)
        {
            if(part is DebugMultitool debugItem)
            {
                GD.Print("Debug Multitool detected, beginning testing ");
                AddPartToShip(debugItem);
                
                RemovePartFromShip(debugItem);
            }
        }
    }


}
