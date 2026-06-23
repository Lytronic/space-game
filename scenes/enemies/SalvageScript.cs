using Godot;
using System;

public partial class SalvageScript : Node2D
{
    BaseEnemyScript parent;

    public ShipPart[] droppedParts;
    private ShipPart[] parentLoot;

    private float wholeRarity;
    private float playerLuck;
	public override void _Ready()
	{
        parent = GetParent<BaseEnemyScript>();

        playerLuck = getPlayerLuckLevel();
        wholeRarity = getWholeRarity();

        dropLoot();
        GD.Print(droppedParts);
	}

	public void dropLoot()
	{
        parentLoot = parent.lootTable;
        GD.Print($"parentLoot table: {parentLoot}"); //debug

        float random = GD.Randf();
        float holyRNG = random + playerLuck;
        int maxRange = (int) Math.Ceiling(holyRNG / wholeRarity);

        droppedParts = new ShipPart[maxRange];
        GD.Print($"random: {random} | playerLuck: {playerLuck} | wholeRarity: {wholeRarity} | max Range: {maxRange}"); //debug


        for (int x = 0; x <= maxRange - 1; x++)
        {
            wholeRarity = getWholeRarity();
            GD.Print($"parentLoot length: {parentLoot.Length}");

            for(int i = 1; holyRNG >= parentLoot[i].rarity;)
            {
                GD.Print($"iterated: {i} | holyRNG: {holyRNG} | parentLoot rarity: {parentLoot[i].rarity} "); //debug

                holyRNG -= parentLoot[i].rarity;
                i++;

                if (holyRNG <= parentLoot[i].rarity || i >= parentLoot.Length)
                {

                    droppedParts[x] = parentLoot[i - 1];
                    parentLoot[i] = null;

                    GD.Print($"break triggered: ({holyRNG <= parentLoot[i].rarity}) ({i >= parentLoot.Length})");

                    break;
                }
            }
        }
    }

    public float getPlayerLuckLevel()
    {
        float luck = 1f;

        //here the playre luck stat will be read

        return luck;
    }

    private float getWholeRarity()
    {
        float wholeRar = 0;

        foreach(ShipPart part  in parent.lootTable)
        {
            if(part != null)
            {
                wholeRar += part.rarity;

            }
        }

        return wholeRar;
    }

}
