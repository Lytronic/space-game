using Godot;
using System;

public partial class DebugMultitool : ShipPart
{
    //non-optional stats
    [Export]
    public new string displayTooltip = "Mayday! Mayday! ... is this fluffin' thing even working?!";
    [Export]
    public new float rarity = 0.1f;
    [Export]
    public new bool isActive = false;
    [Export]
    public new float partWeight = 9.11f;
    private new float thrustIncrease = 2;





    public override void activateEffect() { }

    public override void changeStats(bool add) //changes the player's stats: if add = false --> player stats are decreased | if add = true --> player stats are increased
    {
        int addOrSubtract = addOrSubtractInt(add);

        PlayerVariables.Instance.Thrust += (thrustIncrease * addOrSubtract);
        GD.Print($"increased thrust to : {PlayerVariables.Instance.Thrust}");

        PlayerVariables.Instance.Weight += (partWeight * addOrSubtract);
    }

    public override void generateStats()
    {
        float mult = statRandomness();
        thrustIncrease *= mult;
    }
}
