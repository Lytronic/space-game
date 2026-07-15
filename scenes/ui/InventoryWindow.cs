using Godot;
using System;

public partial class InventoryWindow : Control
{
	private GridContainer _grid;
	private PackedScene _menuShipPartScene = GD.Load<PackedScene>("res://scenes/ship parts/MenuShipPart.tscn");
	
	public override void _Ready()
	{
		_grid = GetNode<GridContainer>("Storage/ScrollContainer/ItemGrid");

		UpdateEntries();
	}

	private void UpdateEntries()
	{
		foreach (var child in _grid.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var part in PlayerVariables.Instance.PlayerCollectedParts)
		{
			var menuPart = _menuShipPartScene.Instantiate<MenuShipPart>();
			menuPart.ShipPart = part;
			
			_grid.AddChild(menuPart);
			GD.Print("part in collected parts");
		}
	}
}
