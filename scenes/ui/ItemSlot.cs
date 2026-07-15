using Godot;
using System;

public partial class ItemSlot : Control
{
	public bool HasItem = false;
	public Vector2I GridPosition;
	
	private Texture2D _textureFree;
	private Texture2D _textureCovered;
	private Texture2D _textureHasItem;
	private TextureRect _texture;

	public override void _Ready()
	{
		_texture = GetNode<TextureRect>("TextureRect");
		_textureFree = ResourceLoader.Load<Texture2D>("res://gfx/gui/menu/storage_slot.png");
		_textureCovered = ResourceLoader.Load<Texture2D>("res://gfx/gui/menu/storage_slot_occupied.png");
		_textureHasItem = ResourceLoader.Load<Texture2D>("res://gfx/gui/menu/storage_slot_blocked.png");
		_texture.Texture = _textureFree;

		GetNode<Label>("Label").Text = GridPosition.ToString();
	}

	public override void _Process(double delta)
	{
		if (HasItem)
		{
			_texture.Texture = _textureHasItem;
		}
		else if (Covered())
		{
			_texture.Texture = _textureCovered;
		}
		else
		{
			_texture.Texture = _textureFree;
		}
	}

	/// <summary>
	/// Whether an item is in the area of the slot, either because
	/// it's already placed or because it's hovering there.
	/// </summary>
	public bool Covered()
	{
		var area = GetNode<Area2D>("Area2D");
		
		// Item slots overlap each other because they're spaces 0px apart
		// So we need to check if it's something else that overlaps, too
		if (area.HasOverlappingAreas())
		{
			var overlaps = area.GetOverlappingAreas();
			foreach (var o in overlaps)
			{
				if (o.GetParent() is not ItemSlot && o.GetNode<MenuShipPart>("../..").Grabbed)
				 return true;
			}
		}

		return false;
	}
}
