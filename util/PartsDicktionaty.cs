using Godot;
using System;
using System.Collections.Generic;

public partial class PartsDicktionaty : Node
{
    public static PartsDicktionaty Instance { get; private set; }


    public override void _Ready()
    {
        Instance = this;
    }

    public static readonly Dictionary<string, ShipPart> ReconstructParts = new()
    {
        
    };
}

