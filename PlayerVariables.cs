using System.Collections.Generic;
using Godot;

public partial class PlayerVariables : Node
{
    public static PlayerVariables Instance { get; private set; }
    public Node Space { get; set; }


    // difficulty level scales all enemy power exponentially
    public int danger_level {  get; private set; } = 1;
    public float luck_stat { get; set; } = 1;

    //durrability stats stuff: armor and shield toughness are scaling stats reducing percentual damage scaling in a power curve | Math.Pow
    public int max_health { get; set; } = 1;
    public int current_health { get; set; } = 1;
    public int max_shield { get; set; } = 1;
    public int curren_shield { get; set; } = 1;
    public float armor_toughness {  get; set; } = 1;
    public float shield_toughness { get; set; } = 1;
    public int shield_regen {  get; set; } = 1; // regen is a flat increase in current shield that gets applied AT THE END OF EVERY FULL SECOND 

    //ship resources 
    public int ammo { get; set; } = 1;
    public float energy { get; set; } = 1;
    public float max_energy { get; set; } = 1;
    public float fuel { get; set; } = 1;
    public float max_fuel { get; set; } = 1;
    public float energy_generation {  get; set; } = 1;



    //this should set max speed and acceleration (thrust against weight)
    public float thrust { get; set; } = 1; //max speed and (acceleration hindered by weight)
    public float weight { get; set; } = 1;
    public float control { get; set; } = 1;// how much the weight influences the acceleration, deceleraton and steering (0.0f - 1.0f)



    //damage will be calculated through percentual damage increase and flat damage multiplication (modifier are being multiplied, mod > 1 --> increase; mod < 1 --> decrease)
    public float damage_modif {  get; set; } = 1;
    public float damge_base { get; set; } = 1; //universial damage buff flat 
    public float phys_damage { get; set; } = 1;
    public float physical_dmg_mod {  get; set; } = 1; // percentage increased physical damage
    public float energy_damage { get; set; } = 1;
    public float energy_dmg_mod { get; set; } = 1; // percentage increased energy damage 

    //this will store all the items the player has in their inventory
    public List<ShipPart> player_active_parts { get; private set; } = new(); // active parts that get activate an effect every time their cooldown is down or under a condition
    public List<ShipPart> player_passive_parts { get; private set; } = new(); // passive parts that only apply an effect on the time they are added to the ship
    public List<ShipPart> player_collected_parts { get; set; } = new(); // basically the stash that the game uses to store all the loot at the end of a round (this gets reset every new round)




    public override void _Ready()
    {
        Instance = this;
    }

    //cahnging difficulty or setting difficult easily (for settings and items)
    public void changeDifficulty(int change, bool setToValue)
    {
        if (setToValue)
        {
            danger_level = change;
        }
        else
        {
            danger_level += change;
        }
    }
    //overload to more easily increase difficulty
    public void changeDifficulty(int change)
    {
        danger_level += change;
    }

    // --------------------- managing the ship parts attatched and not attatched and activae and passive ---------------------

    //adding a part to the ship
    public void addPartToShip(ShipPart part)    
    {
        if(part.isActive)
        {
            player_active_parts.Add(part);
        }
        if(!part.isActive)
        {
            player_passive_parts.Add(part);
            part.changeStats(true); //adds the part's stats to the player's stats
        }
        player_collected_parts.Remove(part);
        //GD.Print($"Part moved!! moved {part} from collection to ship");//debug
    }
    public void removePartFromShip(ShipPart part)
    {
        if (part.isActive)
        {
            player_active_parts.Remove(part);
        }
        if (!part.isActive)
        {
            player_passive_parts.Remove(part);
            part.changeStats(false); //subtracts the part's stats from the player stats
        }
        player_collected_parts.Add(part);
        //GD.Print($"Part moved!! moved {part} from ship to collection "); //debug
    }
    public void addLootToCollection(ShipPart[] array)
    {
        player_collected_parts.AddRange(array);
        GD.Print($"added parts from loot {array} to collection");
        checkForDebugItem(array);
        array = null;
    }
    //if there's a DebugMultitool then I want it to test the other methods too just because I'm not making another testing scenario ^~^
    private void checkForDebugItem(ShipPart[] parts)
    {
        foreach(ShipPart part in parts)
        {
            if(part is DebugMultitool debugItem)
            {
                GD.Print("Debug Multitool detected, beginning testing ");
                addPartToShip(debugItem);
                
                removePartFromShip(debugItem);
            }
        }
    }


}