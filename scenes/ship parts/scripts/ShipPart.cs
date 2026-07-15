using Godot;
using System;
using System.Collections;
using System.Reflection.Metadata.Ecma335;

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
	[Export] public virtual Texture2D MenuTexture { get; set; }

	[Export] public virtual string SpriteTexturePath { get; set; }
	[Export] public virtual string MenuTexturePath { get; set; }

	//variables needed to recreate the item from save
	[Export] public virtual string type { get; set; }
	[Export] public virtual float randomness { get; set; } = -1f;



	/// <value>The coordinates in the BuildMenu grid (if applicable)</value>
	public Vector2I GridPosition;

	/// <value>Part form in the BuildMenu grid as a 3x3 BitArray.</value>
	public virtual BitArray Shape { get; set; } = new(new bool[]{
		true, true, true,
		true, true, true,
		true, true, true
	});
   
	public void Initialize()
	{
		danger = PlayerVariables.Stats.DangerLevel;
		loadSprite();
		
	}
	public virtual void loadSprite()
	{
		SpriteTexture = GD.Load<Texture2D>(SpriteTexturePath);
		MenuTexture = GD.Load<Texture2D>(MenuTexturePath);
	}
	public virtual void activateEffect() { }

	public virtual void changeStats(bool add) //changes the player's stats: if add = false --> player stats are decreased | if add = true --> player stats are increased
	{
		//int addOrSubtract = add ? 1 : -1;

		//PlayerVariables.Instance.playerStat += (partStat * addOrSubtract)  <--- this is very important information but just a schema to show how to do it
		
		//here the weapon adds a child to the partsManager for actual shooting
	}
	public int addOrSubtractInt(bool add) // support method to streamline applying stats in subclasses
	{
		if(add)
		{
			return 1;
		}
		else { return -1; }
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

	public SavedPart SavePartVariables()
	{
		SavedPart saved = new SavedPart(type, randomness);
		return new SavedPart(type, randomness);
	}

}
