using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class BaseEnemyScript : Node
{
    //here will be the public loot table that Salvage uses to choose a drop from
    //public loottabel die type salvage ist

    private const int amountOfLoot = 5;
    [Export]
    public ShipPart[] lootTable { get; set; }

    public override void _EnterTree()
    {
        //here the loot table will be created before all the children are loaded
        createLootTable();
        generateDroppedstats();

    }
    public override void _Ready()
	{
    }

	public override void _Process(double delta)
	{

	}

    private void createLootTable()
    {
        // if you don't order these by smallest rarity first, I'm murdering you; the first one must be 'null' unlesss the enemy has guaranteed drops
        lootTable = new ShipPart[amountOfLoot]
        {
            null, new ShipPart(), new ShipPart(), new ShipPart(), new ShipPart(),};
    }
    private void generateDroppedstats()
    {
        foreach (ShipPart part in lootTable)
        {
            if(part != null)
            {
                part.generateStats();
            }
        }
        GD.Print("Generated Stats for dropped loot ");
    }

    private void enemyDie()
    {
        Salvage salvageNode = GetNode<SalvageScript>;
        salvageNode.dropLoot
    }

}

