using Godot;
using System;

[GlobalClass]
public partial class BaseProjectile : Area2D
{
    //base parameters
    [Export] public float projectileDamage = 1;
    [Export] public float projectileSpeed = 1;
    [Export] public Vector2 projectileDirection;
    [Export] public bool malicious = true;
    public override void _EnterTree()
    {
        //connect signal 
        BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += MoveInPattern(delta) * (float)(projectileSpeed * delta);
    }
    public void OnBodyEntered(Node2D body)
    {
        GD.Print("Collided!!"); //debug
        if (body is BaseEnemy enemy)
        {
            enemy.enemyTakeDamage(projectileDamage);
        }

        if (malicious)
        {
            //now query for player dmage
        }
        else
        {
            
        }

        this.QueueFree();
    }
    public void spawnProjectile(float damage , Vector2 direction)
    {
        //this puts the projectile as an independent item in the scene and into the tree and lets it do its thing
        projectileDamage = damage;
        PlayerVariables.Instance.Space.AddChild(this);
        projectileDirection = direction;
    }
    public virtual Vector2 MoveInPattern(double time)
    {
        
        //changes projectileDirection according to the preprogrammed and posibly custom movement pattern
        return projectileDirection;
    }
}
