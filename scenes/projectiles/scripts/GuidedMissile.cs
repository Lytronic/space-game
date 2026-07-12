using Godot;
using System;

/// <summary>
/// A missile that locks onto a target and chases it.
/// </summary>
public partial class GuidedMissile : BaseMissile
{
	/// <value>Describes how quickly the missile can turn.</value>
	[Export] public float Agileness = 100.0f;

	public CharacterBody2D Target;

	public override Vector2 MoveInPattern(double time)
	{
		AcquireTarget();
		if (Target == null || !IsInstanceValid(Target))
		{
			return Direction;
		}
		
		Vector2 targetVec = (Target.GlobalPosition - GlobalPosition).Normalized();

		return (Direction + targetVec * (float)time * Agileness).Normalized();
	}

	private void AcquireTarget()
	{
		if (Target == null)
		{
			if (_owner is BaseEnemy)
			{
				Node scene = GetTree().CurrentScene;
				Target = scene.GetNode<Player>("Player");
			}
			else
			{
				var entities = PlayerVariables.Space.GetChildren();

				foreach(var entity in entities)
				{
					// select the closest enemy among all entities
					if (entity is BaseEnemy enemy)
					{
						Target ??= enemy;
						Target = (enemy.GlobalPosition - GlobalPosition).Length()
							< (Target.GlobalPosition - GlobalPosition).Length() ? enemy : Target; 
					}
				}
			}
		}
	}
}
