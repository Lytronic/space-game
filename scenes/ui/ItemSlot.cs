using Godot;
using System;

public partial class ItemSlot : Control
{
	public bool HasItem = false;
	
	private Texture2D _textureFree;
	private Texture2D _textureOccupied;
	private TextureRect _texture;

	public override void _Ready()
	{
		_texture = GetNode<TextureRect>("TextureRect");
		_textureFree = ResourceLoader.Load<Texture2D>("res://gfx/gui/menu/storage_slot.png");
		_textureOccupied = ResourceLoader.Load<Texture2D>("res://gfx/gui/menu/storage_slot_occupied.png");
	}

	public override void _Process(double delta)
	{
		_texture.Texture = Covered() ? _textureOccupied : _textureFree;
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
				if (o.GetParent() is not ItemSlot) return true;
			}
		}

		return false;
	}
}
