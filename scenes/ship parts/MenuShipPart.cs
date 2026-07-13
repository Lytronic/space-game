using Godot;
using System;
using System.Linq;

public partial class MenuShipPart : Control
{
	public ShipPart ShipPart;

	private bool _hovered = false;
	private bool _grabbed = false;
	private bool _inGrid = false;
	private TextureRect _gridTexture;
	private TextureRect _menuTexture;
	private Control _areas;
	private Vector2 _spawnPos;
	private Node _formerParent;

	public override void _Ready()
	{
		_gridTexture = GetNode<TextureRect>("GridTexture");
		_menuTexture = GetNode<TextureRect>("MenuTexture");
		_areas = GetNode<Control>("Anchors");
		_gridTexture.Texture = ShipPart.SpriteTexture;
		_gridTexture.Hide();
		_menuTexture.Texture = ShipPart.MenuTexture;

		_spawnPos = new Vector2(1200.0f, 500.0f);

		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				if (ShipPart.Shape[i,j])
				{
					Area2D area = new()
					{
						Position = new Vector2(i, j) * BuildMenu.GridSpacing
							+ new Vector2(0.5f, 0.5f) * (BuildMenu.GridSpacing / 2),
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
		
		MouseEntered += () => {
			_gridTexture.Scale *= 1.125f;
			_hovered = true;
		};
		
		MouseExited += () => {
			_gridTexture.Scale /= 1.125f;
			_hovered = false;
		};
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

		if (_inGrid)
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

			_inGrid = false;
			PlayerVariables.Instance.RemovePartFromShip(ShipPart);
		}
		else
		{
			// free the node from its parent so it can be dragged out of it
			_formerParent = GetParent();
			Reparent(GetNode("/root"));
		}
	}

	public void Drop()
	{
		_grabbed = false;
		if (!CanDrop())
		{
			_gridTexture.Hide();
			_menuTexture.Show();
			Reparent(_formerParent);
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

			_inGrid = true;
			PlayerVariables.Instance.AddPartToShip(ShipPart);
		}
	}
}
