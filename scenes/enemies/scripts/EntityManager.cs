using Godot;
using System;

public partial class EntityManager : Node2D
{
	//this scene here is only to give the moving parts in the game an immobile scene to spawn in



	public override void _EnterTree()
	{
		PlayerVariables.Space = this;
	}
}
