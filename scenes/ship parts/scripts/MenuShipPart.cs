using Godot;
using System;
using System.Collections;
using System.Linq;

public partial class MenuShipPart : Control
{
	[Export]
	public ShipPart ShipPart;
	public bool InGrid = false;
	public Node Inventory;

	private bool _hovered = false;
	private bool _grabbed = false;
	private TextureRect _gridTexture;
	private TextureRect _menuTexture;
	private Control _areas;

	public override void _Ready()
    {
        _gridTexture = GetNode<TextureRect>("GridTexture");
        _menuTexture = GetNode<TextureRect>("MenuTexture");
        _areas = GetNode<Control>("Anchors");
        _gridTexture.Texture = ShipPart.SpriteTexture;
        _menuTexture.Texture = ShipPart.MenuTexture;
        TooltipText = ShipPart.displayTooltip;

        if (InGrid)
        {
            _menuTexture.Hide();
        }
        else
        {
            _gridTexture.Hide();
        }

        SpawnAreas();

        MouseEntered += () =>
        {
            _gridTexture.Scale *= 1.125f;
            _hovered = true;
        };

        MouseExited += () =>
        {
            _gridTexture.Scale /= 1.125f;
            _hovered = false;
        };
    }

    /// <summary>
    /// Spawn the Area2Ds used to detect which part of the ShipPart is inside a slot.
    /// </summary>
    private void SpawnAreas()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (ShipPart.Shape[3 * j + i])
                {
                    Area2D area = new()
                    {
                        Position = new Vector2(i, j) * BuildMenu.GridSpacing
                            + new Vector2(0.5f, 0.5f) * BuildMenu.GridSpacing,
                    };

                    area.AddChild(new CollisionShape2D()
                    {
                        Shape = new RectangleShape2D()
                        {
                            Size = new Vector2(0.0f, 0.0f)
                        }
                    });

                    _areas.AddChild(area);
                }
            }
        }

		Vector2 offset = new(0.0f, 0.0f);
    	
		// Adjusting Area2D positions for parts that aren't centred (e.g. 2x3)

		int n = 3;
		bool[] rowHasTrue = new bool[n];
        bool[] colHasTrue = new bool[n];

        for (int i = 0; i < n; i++)
        {
            // row i
            bool rowAny = false;
            for (int j = 0; j < n; j++)
                rowAny |= ShipPart.Shape[i * n + j];
            rowHasTrue[i] = rowAny;

            // column i
            bool colAny = false;
            for (int j = 0; j < n; j++)
                colAny |= ShipPart.Shape[j * n + i];
            colHasTrue[i] = colAny;
        }

		// check for empty rows/columns
        if (!rowHasTrue[0])
        {
        	offset.Y -= BuildMenu.GridSpacing / 2;
        }

        if (!rowHasTrue[2])
        {
        	offset.Y += BuildMenu.GridSpacing / 2;
        }

        if (!colHasTrue[0])
        {
        	offset.X -= BuildMenu.GridSpacing / 2;
        }

        if (!colHasTrue[2])
        {
        	offset.X += BuildMenu.GridSpacing / 2;
        }

		// apply final offset
		foreach (Area2D area in _areas.GetChildren().Cast<Area2D>())
    	{
    		area.Position += offset;
    	}
    }

    public override void _Process(double delta)
	{
		if (_grabbed)
		{
			GlobalPosition = GetGlobalMousePosition() - _gridTexture.Size / 2;
		}

		
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent &&
			mouseEvent.ButtonIndex == MouseButton.Left &&
			_hovered)
		{
			if (mouseEvent.Pressed)
			{
				Grab();
			}
			else
			{
				Drop();
			}
		}
	}

	public bool CanDrop()
	{
		var areas = _areas.GetChildren();

		foreach (Area2D area in areas.Cast<Area2D>())
		{
			if (!area.HasOverlappingAreas()) return false;

			foreach (var overlap in area.GetOverlappingAreas())
			{
				if (overlap.GetParent() is ItemSlot slot && slot.HasItem) return false;
			}
		}
		return true;
	}

	public void Grab()
	{
		_grabbed = true;
		_gridTexture.Show();
		_menuTexture.Hide();

		if (InGrid)
		{
			foreach (Area2D area in _areas.GetChildren().Cast<Area2D>())
			{
				foreach (var overlap in area.GetOverlappingAreas())
				{
					var parent = overlap.GetParent();
					if (parent is ItemSlot slot)
					{
						slot.HasItem = false;
					}
				}
			}

			InGrid = false;
			PlayerVariables.Instance.RemovePartFromShip(ShipPart);
			ShipPart.GridPosition = new(-1, -1);
		}
		else
		{
			// free the node from its parent so it can be dragged out of it
			Inventory = GetParent();
			Reparent(GetNode("/root/BuildMenu"));
		}
	}

	public void Drop()
	{
		_grabbed = false;
		if (!CanDrop())
		{
			_gridTexture.Hide();
			_menuTexture.Show();
			Reparent(Inventory);
		}
		else
		{
			foreach (Area2D area in _areas.GetChildren().Cast<Area2D>())
			{
				foreach (var overlap in area.GetOverlappingAreas())
				{
					var parent = overlap.GetParent();
					if (parent is ItemSlot slot)
					{
						slot.HasItem = true;
					}
				}
			}

			// The top-leftmost slot touched by the part defines its position in the grid
			var indexSlot = (ItemSlot)((Area2D)_areas.GetChild(0)).GetOverlappingAreas()[0].GetParent();
			ShipPart.GridPosition = indexSlot.GridPosition;
			GlobalPosition = ((Control)indexSlot.GetParent()).GlobalPosition +
				(new Vector2(indexSlot.GridPosition.X, indexSlot.GridPosition.Y) * BuildMenu.GridSpacing);

			InGrid = true;
			PlayerVariables.Instance.AddPartToShip(ShipPart);
		}
	}
}
