using Godot;
using System;

[GlobalClass]
public partial class EnemySalvage : Node
{
	BaseEnemy parent;

	public ShipPart[] droppedParts;
	private ShipPart[] possibleLoot;

	private float wholeRarity;
	private float playerLuck;
	public override void _Ready()
	{
		parent = GetParent<BaseEnemy>();
	
	}

	public void dropLoot()
	{
		possibleLoot = parent.lootTable;
        //GD.Print($"parentLoot table: {possibleLoot}"); //debug

        playerLuck = getPlayerLuckLevel();
        wholeRarity = getWholeRarity();

        float random = GD.Randf();
		float lootStrength = random + playerLuck;

		int maxRange = 0;

		if((lootStrength / wholeRarity) >= 1)
		{
			maxRange = maxRange = Math.Max(1, (int)Math.Floor(lootStrength / Math.Max(wholeRarity, 0.0001f))); 
		}
		else 
		{
			maxRange = 1;
		}

		droppedParts = new ShipPart[maxRange];
		GD.Print($"random: {random} | playerLuck: {playerLuck} | wholeRarity: {wholeRarity} | max Range: {maxRange}"); //debug

		// here the ar
		for (int x = 0; x < maxRange; x++)
		{
			if (possibleLoot == null || possibleLoot.Length == 1) { break; }
			GD.Print($"possibleLoot length: {possibleLoot.Length}");  //debug

			for(int i = 1; lootStrength >= possibleLoot[i].rarity;)
			{
				GD.Print($"iteration: {i} | lootStrength: {lootStrength} | possibleLoot rarity: {possibleLoot[i].rarity} "); //debug

				lootStrength -= possibleLoot[i].rarity;
				i++; // <--- IMPORTANT!! the index 'i' is at this point BIGGER than the item of this current iteration

				//breaks as soon as the rarest (last) object in the loottable is already reached in this iteration, the drops it -- don't forget that the length of an aray is 1 bigger than its last index
				if (i >= possibleLoot.Length)
				{
					droppedParts[x] = possibleLoot[possibleLoot.Length - 1];
					removeFromPossibleLoot(possibleLoot.Length - 1);
					GD.Print($"rarest loot reached, breaking - dropped: {droppedParts[x]} | parent: {possibleLoot[possibleLoot.Length - 1]} ");//only correct if dropped: -dropped obj- | parent: -moved up obj- !!!
					break;
				}
				//if the loot strengt isn't greater than the next possible loot, then drop the loot of this iteration
				if (lootStrength < possibleLoot[i].rarity)
				{
					droppedParts[x] = possibleLoot[i - 1];
					removeFromPossibleLoot(i - 1);
					GD.Print($"loot strength too low for next item, breaking - dropped: {droppedParts[x]} | parent: {possibleLoot[possibleLoot.Length - 1]} "); //debug
					break;
				}
			}
			GD.Print($"compleded dropping process, dropped parts: {droppedParts}");   //debug
		}
		PlayerVariables.Instance.AddLootToCollection(droppedParts);
	}

	private float getPlayerLuckLevel()
	{
		float luck = PlayerVariables.Instance.LuckStat;
		return luck;
	}

	private float getWholeRarity()
	{
		float wholeRar = 0;

		foreach(ShipPart part  in possibleLoot)
		{
			if(part != null)
			{
				wholeRar += part.rarity;
				GD.Print($"getWholeRarity - part rarity: {part.rarity}");
			}
		}

		return wholeRar;
	}

    private void removeFromPossibleLoot(int indexToRemove)
    {
        ShipPart[] newLoot = new ShipPart[possibleLoot.Length - 1];

        int j = 0;

        for (int i = 0; i < possibleLoot.Length; i++)
        {
            if (i == indexToRemove)
                continue;

            newLoot[j++] = possibleLoot[i];
        }

        possibleLoot = newLoot;
    }
    
}
