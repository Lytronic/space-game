using Godot;

public partial class BuildMenu : Node2D
{
	PlayerVariables stats = PlayerVariables.Instance;

    private PackedScene _placedShipPartScene = GD.Load<PackedScene>("res://scenes/ship parts/PlacedShipPart.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		GD.Print(stats.player_collected_parts.Count);

		// Iterate the components the player has collected
		foreach (ShipPart partData in stats.player_collected_parts)
		{
			partData.SpriteTexture = GD.Load<Texture2D>("res://gfx/gui/parts/weapon_emp.png");

            PlacedShipPart placedPart = _placedShipPartScene.Instantiate<PlacedShipPart>();
            placedPart.Initialize(partData);
            AddChild(placedPart);

            int rx = GD.RandRange(-5, 5);
            int ry = GD.RandRange(-5, 5);
            placedPart.Position = new Vector2(864 + rx, 324 + ry);
        }
	}
    public void OpenMainScene()
    {
        GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
    }
}
