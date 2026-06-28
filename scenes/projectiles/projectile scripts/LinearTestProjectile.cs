using Godot;
using System;

public partial class LinearTestProjectile : BaseProjectile
{
    
    public override void _Ready()
    {
        projectileSpeed = 3;
        projectileDirection = Vector2.Right;
        projectileDamage = 99;
    }
    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += MoveInPattern(delta) * projectileSpeed;
    }

    public override Vector2 MoveInPattern(double time)
    {
        return projectileDirection * (float)time;
    }
}
