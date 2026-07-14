using System;
using System.Collections.Generic;
using Godot;
using MemoryPack;

/// <summary>
/// Data to be saved in the save database.
/// Stats is the most significant as it contains most game state.
/// However, ship parts need to be serialised separately as they are Godot resources,
/// which don't work with MemoryPack.
/// This is why we store them separately in this wrapper object.
/// We also store the date on which the game was saved and its ID in the DB.
/// </summary>
[MemoryPackable]
public partial struct SaveData
{
	public int Id;
	public DateTime SavedTime;
	
	public Stats Stats;
	public List<SavedPart> ActiveParts;
    public List<SavedPart> PassiveParts;
    public List<SavedPart> CollectedParts;
}

/// <summary>
/// Ship parts are serialised separately into this object
/// </summary>
[MemoryPackable]
public partial class SavedPart
{
	public string PartType { get; }
	public float PartRandomness { get; }
	public Vector2I GridPosition;

    public SavedPart (string partType , float partRandomness)
	{
		PartType = partType;
		PartRandomness = partRandomness;
	}

}

