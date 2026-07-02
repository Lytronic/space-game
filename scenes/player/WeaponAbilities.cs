using Godot;
using System;

public partial class WeaponAbilities : HBoxContainer
{
	[Export] public Texture2D SpriteAbilityARC;
	[Export] public Texture2D SpriteAbilityLASER;
	[Export] public Texture2D SpriteAbilityMISSILE;
	[Export] public Texture2D SpriteAbilityPLASMA;
	[Export] public Texture2D SpriteAbilityEMP;
	[Export] public Texture2D SpriteAbilityTORPEDO;
	[Export] public Texture2D SpriteAbilityCANNON;
	
	public override void _Ready()
	{
		if(true)
		{
			/*var Ability1 = new TextureRect();
			Ability1.Texture = SpriteAbilityPLASMA;
			ExpandMode = TextureRect.ExpandModeEnum.ExpandIgnoreSize;
			AddChild(Ability1);*/
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	// sample code for the abilities:
	/*
	
	*/
	
}
