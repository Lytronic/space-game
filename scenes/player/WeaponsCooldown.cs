using Godot;
using System;

public partial class WeaponsCooldown : HBoxContainer
{

	Label[] Cooldowns;

	public override void _Ready()
	{
		Cooldowns = new Label[8];
		for (int i = 0; i < Cooldowns.Length; i++)
		{
			Cooldowns[i] = GetChild(i) as Label;
		}
		for (int i = 0; i < Cooldowns.Length; i++)
		{
			Cooldowns[i].Text = $"{i}";
		}

	}
	public override void _Process(double delta)
	{

	}

	public void CallCooldown(int cooldownNumber, int cooldownLength)
	{
		if(Cooldowns[cooldownNumber] != null)
		{
			Cooldowns[cooldownNumber].Show();
			for (int i = cooldownLength; i > 0; i--)
			{
				Cooldowns[cooldownNumber].Text = $"{i}";
			}
			Cooldowns[cooldownNumber].Hide();
		}
		else
		{
			GD.PrintErr("ERROR! Cooldowns[] element out of bounds or null! Cooldown Number: " + cooldownNumber);
		}
	}
}
