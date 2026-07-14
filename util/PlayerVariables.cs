using System.Collections.Generic;
using Godot;
using MemoryPack;
using Microgravity.util;

/// <summary>
/// Most of the serialisable data that describes the game state.
/// This object must be kept serialisable, which is why it's its separate type.
/// Godot classes such as Node cannot be serialised easily with MemoryPack.
/// However, all primitive types and types declared [MemoryPackable] can be added as members.
/// </summary>
[MemoryPackable]
public partial class Stats
{
	// The player's current score
	public int Score = 0;

	// The current round the player is in
	public int Round = 0;

	// difficulty level scales all enemy power exponentially
	public int DangerLevel {  get; set; } = 0;
	public float LuckStat { get; set; } = 1;

	//durrability stats stuff: armor and shield toughness are scaling stats reducing percentual damage scaling in a power curve | Math.Pow
	public float MaxHealth { get; set; } = 100.0f;
	public float CurrentHealth { get; set; } = 100.0f;
	public float MaxShield { get; set; } = 100.0f;
	public float CurrentShield { get; set; } = 100.0f;
	public float ArmorToughness {  get; set; } = 1;
	public float ShieldToughness { get; set; } = 1;
	public int ShieldRegen {  get; set; } = 1; // regen is a flat increase in current shield that gets applied AT THE END OF EVERY FULL SECOND 

	public int RegenCooldown {  get; set; } = 1;
	
	//ship resources 
	public int Ammo { get; set; } = 1;
	public float Energy { get; set; } = 1;
	public float MaxEnergy { get; set; } = 100.0f;
	public float Fuel { get; set; } = 1;
	public float MaxFuel { get; set; } = 1;
	public float EnergyGeneration {  get; set; } = 1;

	//this should set max speed and acceleration (thrust against weight)
	public float Thrust { get; set; } = 1; //max speed and (acceleration hindered by weight)
	public float Weight { get; set; } = 1;
	public float Control { get; set; } = 1;// how much the weight influences the acceleration, deceleraton and steering (0.0f - 1.0f)

	//damage will be calculated through percentual damage increase and flat damage multiplication (modifier are being multiplied, mod > 1 --> increase; mod < 1 --> decrease)
	public float DamageModif {  get; set; } = 1;
	public float DamageBase { get; set; } = 1; //universial damage buff flat 
	public float PhysDamage { get; set; } = 1;
	public float PhysicalDmgMod {  get; set; } = 1; // percentage increased physical damage
	public float EnergyDamage { get; set; } = 1;
	public float EnergyDmgMod { get; set; } = 1; // percentage increased energy damage 

	// Inventory
    public int InvWidth = 7;
    public int InvHeight = 9;
}

/// <summary>
/// The singleton to hold all kinds of player state that requires easy access from anywhere.
/// </summary>
public partial class PlayerVariables : Node
{
	public static PlayerVariables Instance { get; private set; }
	public static Node Space { get; set; }
	public static Stats Stats;
   
	// TODO: Serialise ShipParts, probably by storing IDs about them in Stats 
	//this will store all the items the player has in their inventory
	public List<ShipPart> PlayerActiveParts { get; set; } = []; // active parts that get activate an effect every time their cooldown is down or under a condition
	public List<ShipPart> PlayerPassiveParts { get; set; } = []; // passive parts that only apply an effect on the time they are added to the ship
	public List<ShipPart> PlayerCollectedParts { get; set; } = []; // basically the stash that the game uses to store all the loot at the end of a round (this gets reset every new round)

	public override void _Ready()
	{
		GD.Print($"New PlayerVariables object added to tree, setting Instance to {this}");
		Instance = this;
		Stats = new();
	}

	/// <summary>
	/// Restores the persistent player state for a fresh game run.
	/// This is done by creating a new PlayerVariables object and deleting the old one
	/// to make sure no state from the previous game carries over.
	/// </summary>
	public void ResetRun()
	{
		// make sure the name isn't PlayerVariables so there's no conflict with the new one
		Instance.Name = "TO_BE_DELETED";
	
		// Add the new object to the tree, Instance will be set in _Ready()
		GetNode("/root").AddChild(new PlayerVariables(){ Name = "PlayerVariables" });

		this.QueueFree();
	}

	/// <summary>
	/// Load a saved SaveData object and replace the current state.
	/// </summary>
	public static void LoadFromSave(int id)
	{
		var data =  DB.LoadGame(id);

		Stats = data.Stats;
	}
	
	/// <summary>
	/// Applies shield mitigation first, then armor mitigation to remaining hull damage.
	/// </summary>
	public void ApplyDamage(float amount)
	{
		if (amount <= 0.0f)
			return;

		float remainingDamage = amount;
		float shieldToughness = Mathf.Max(0.1f, Stats.ShieldToughness);
		float armorToughness = Mathf.Max(0.1f, Stats.ArmorToughness);

		if (Stats.CurrentShield > 0.0f)
		{
			float shieldDamage = remainingDamage / shieldToughness;
			float absorbedShield = Mathf.Min(Stats.CurrentShield, shieldDamage);
			Stats.CurrentShield -= absorbedShield;
			remainingDamage -= absorbedShield * shieldToughness;
		}

		if (remainingDamage > 0.0f)
			Stats.CurrentHealth = Mathf.Clamp(Stats.CurrentHealth - remainingDamage / armorToughness, 0.0f, Stats.MaxHealth);
	}

	public void RegenShield()
	{
		if(Stats.RegenCooldown > 0)
		{
			Stats.RegenCooldown -= 1;
		}
		else if(Stats.CurrentShield < Stats.MaxShield && Stats.Energy > 0)
		{
			Stats.CurrentShield += Stats.ShieldRegen;
			Stats.Energy -= Stats.ShieldRegen;
			GD.Print("Regening Shield");
			if(Stats.CurrentShield > Stats.MaxShield)
			{
				Stats.CurrentShield = Stats.MaxShield;
			}
			if(Stats.Energy < 0)
			{
				Stats.Energy = 0;
			}
		}
	}

	public void RegenEnergy()
	{
		if(Stats.Energy < Stats.MaxEnergy)
		{
			Stats.Energy += Stats.EnergyGeneration;
			if(Stats.Energy > Stats.MaxEnergy)
			{
				Stats.Energy = Stats.MaxEnergy;
			}
		}
	}


	// IMPORTANT! This function only calculates how much energy an action uses, check if Energy != 0 before calling it!
	public void UseEnergy(float usage)
	{
		if(Stats.Energy > 0)
		{
			Stats.Energy -= usage;
		}
		if(Stats.Energy < 0)
		{
			Stats.Energy = 0;
		}
	}

	//changing difficulty or setting difficult easily (for settings and items)
	public void ChangeDifficulty(int change, bool setToValue)
	{
		if (setToValue)
		{
			Stats.DangerLevel = change;
		}
		else
		{
			Stats.DangerLevel += change;
		}
	}

	//overload to more easily increase difficulty
	public void ChangeDifficulty(int change)
	{
		Stats.DangerLevel += change;
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
		GD.Print($"Part moved!! moved {part} from ship to collection "); //debug
	}

	public void AddLootToCollection(ShipPart[] array)
	{
		PlayerCollectedParts.AddRange(array);

		GD.Print($"added parts from loot {array} to collection");
		//CheckForDebugItem(array);
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
    // each integer represents the amount of weapons equipped for that type of weapon, this will be changed by the equipped ShipParts of type Weapon
    public int[] WeaponList = new int[10]
    {
        0,  // this index (0) represents the amount of start weapon the player has equipped
		0,  // Arc
		0,  // Cannon
		0,  // Coil
		0,  // EMP
		0,  // Laser
		0,  // Missile
		0,  // Plasma
		0,  // Rail
		0   // Torpedo
	};
    public void SaveGame()
	{
		SaveData save = new SaveData();
		save.Stats = Stats;

		//deconstruct the lists of parts into serializable bits
		List<SavedPart> activeParts = new List<SavedPart>();
		foreach(ShipPart part in PlayerActiveParts)
		{
			activeParts.Add(part.SavePartVariables());
		}

		List<SavedPart> passiveParts = new List<SavedPart>();
        foreach (ShipPart part in PlayerPassiveParts)
        {
            passiveParts.Add(part.SavePartVariables());
        }

        List<SavedPart> collectedParts = new List<SavedPart>();
        foreach (ShipPart part in PlayerCollectedParts)
        {
            collectedParts.Add(part.SavePartVariables());
        }

		//put the serialized lists of shipParts into DB
		save.ActiveParts.AddRange(activeParts);
		save.PassiveParts.AddRange(passiveParts);
		save.CollectedParts.AddRange(collectedParts);

		DB.SaveGame("name of save" , save);
    }
}
