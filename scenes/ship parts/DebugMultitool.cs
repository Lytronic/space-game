using Godot;
using System;

public partial class DebugMultitool : ShipPart
{
	//non-optional stats
	[Export]
	public override string displayTooltip { get; set; }  = "Mayday! Mayday! ... is this fluffin' thing even working?!";
	[Export]
	public override float rarity { get; set; } = 0.1f;
	[Export]
	public override bool isActive { get; set; } = false;
	[Export]
	public override float partWeight { get; set; } = 9.11f;
	private float thrustIncrease = 2;





	public override void activateEffect() { }

	public override void changeStats(bool add) //changes the player's stats: if add = false --> player stats are decreased | if add = true --> player stats are increased
	{
		int addOrSubtract = addOrSubtractInt(add);

		PlayerVariables.Stats.Thrust += (thrustIncrease * addOrSubtract);
		GD.Print($"increased thrust to : {PlayerVariables.Stats.Thrust}");

		PlayerVariables.Stats.Weight += (partWeight * addOrSubtract);
	}

	public override void generateStats()
	{
		float mult = statRandomness();
		thrustIncrease *= mult;
	}
}
