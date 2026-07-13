using Godot;
using System;
using System.Linq;

public partial class MenuShipPart : Control
{
	public ShipPart ShipPart;

	private bool _hovered = false;
	private bool _grabbed = false;
	private bool _inGrid = false;
	private TextureRect _texture;
	private Control _areas;
	private Vector2 _spawnPos;

	public override void _Ready()
	{
		_texture = GetNode<TextureRect>("TextureRect");
		_areas = GetNode<Control>("Anchors");
		_texture.Texture = ShipPart.SpriteTexture;

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
			_texture.Scale *= 1.125f;
			_hovered = true;
		};
		
		MouseExited += () => {
			_texture.Scale /= 1.125f;
			_hovered = false;
		};
	}

	public override void _Process(double delta)
	{
		if (_grabbed)
		{
			GlobalPosition = GetGlobalMousePosition() - _texture.Size / 2;
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

			PlayerVariables.Instance.RemovePartFromShip(ShipPart);
		}
	}

	public void Drop()
	{
		_grabbed = false;
		if (!CanDrop())
		{
			GlobalPosition = _spawnPos;
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
