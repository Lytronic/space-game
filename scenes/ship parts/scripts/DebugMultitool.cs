using Godot;
using System;
using System.Collections;

public partial class DebugMultitool : ShipPart
{
	//non-optional stats
	[Export] public override string displayTooltip { get; set; }  = "Mayday! Mayday! ... is this fluffin' thing even working?!";
	[Export] public override float rarity { get; set; } = 0.1f;
	[Export] public override bool isActive { get; set; } = false;
	[Export] public override string type { get; set; } = "DebugMultitool";
	[Export] public override float partWeight { get; set; } = 9.11f;

	[Export] public override Texture2D SpriteTexture { get; set; }
	[Export] public override Texture2D MenuTexture { get; set; }

	[Export] public override string SpriteTexturePath { get; set; } = "res://gfx/gui/parts/weapon_torpedo.png";
	[Export] public override string MenuTexturePath { get; set; } = "res://gfx/gui/construction/icon/weaponry/weapon_torpedo.png";

	//type specific values
	private float thrustIncrease = 2;

	//type specific texture
	//public override Texture2D SpriteTexture { get; set; } = 

	/// <value>Part form in the BuildMenu grid as a 3x3 BitArray.</value>
	public BitArray Shape = new(new bool[]{
		true, true, true,
		true, true, true,
		true, true, true
	});



	public override void activateEffect() { }

	public override void changeStats(bool add) //changes the player's stats: if add = false --> player stats are decreased | if add = true --> player stats are increased
	{
		int addOrSubtract = addOrSubtractInt(add);

		PlayerVariables.Stats.Thrust += (thrustIncrease * addOrSubtract);
		GD.Print($"increased thrust to : {PlayerVariables.Stats.Thrust}");

		PlayerVariables.Stats.Weight += (partWeight * addOrSubtract);
		GD.Print(PlayerVariables.Instance.WeaponList[0]);
	}

	public override void generateStats()
	{
		GenerateStatRandomness();
		
		float mult = randomness;
		thrustIncrease *= mult;
		GD.Print($"random: {mult}");
	}
}
