using Godot;
using System;

public partial class BuildUi : Control
{
	public void _on_button_pressed()
	{
		GetTree().CurrentScene.Call("OpenMainScene");
	}
}
