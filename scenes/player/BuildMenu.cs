using Godot;
using Godot.Collections;

public partial class BuildMenu : Control
{
	PlayerVariables vars = PlayerVariables.Instance;

	public static readonly float GridSpacing = 90.0f;
	/// <value>Reference for items to know where to go when they're pulled out of the grid.</value>
	[Export] public GridContainer InventoryRef;

	/// <value>Maximum width/height a ship part can have (See ShipPart.cs)</value>
	public static readonly int PartGridSize = 3;

	private PackedScene _menuShipPartScene = ResourceLoader.Load<PackedScene>("res://scenes/ship parts/MenuShipPart.tscn");
	private PackedScene _itemSlotScene = ResourceLoader.Load<PackedScene>("res://scenes/ui/ItemSlot.tscn");
	private GridContainer _grid;

	public override void _Ready()
	{
		GetNode<Button>("Button").Pressed += BackToGame;
		_grid = GetNode<GridContainer>("Grid");
		
		// Add item slots to scene
		_grid.Columns = PlayerVariables.Stats.InvWidth;
		for (int y = 0; y < PlayerVariables.Stats.InvHeight; y++)
		{
			for (int x = 0; x < PlayerVariables.Stats.InvWidth; x++)
			{
				var slot = _itemSlotScene.Instantiate<ItemSlot>();
				slot.GridPosition = new(x, y);

				_grid.AddChild(slot);
			}
		}

		// add equipped parts to grid where they were the last time this menu was open
		foreach (var part in PlayerVariables.Instance.PlayerPassiveParts)
		{
			var menuPart = _menuShipPartScene.Instantiate<MenuShipPart>();
			part.SpriteTexture = GD.Load<Texture2D>("res://gfx/gui/parts/weapon_emp.png");
			part.MenuTexture = GD.Load<Texture2D>("res://gfx/gui/construction/icon/weaponry/weapon_arc.png");
			menuPart.ShipPart = part;

			menuPart.Position = _grid.GlobalPosition + (new Vector2(part.GridPosition.X, part.GridPosition.Y) * GridSpacing);
			menuPart.InGrid  = true;
			menuPart.Inventory = InventoryRef;

			AddChild(menuPart);
		}
	}
	
	public void BackToGame()
	{
		GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
		QueueFree();
	}
}
