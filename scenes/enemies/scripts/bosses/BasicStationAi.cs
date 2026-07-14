using System;
using System.Linq;
using Godot;
using Godot.Collections;


public partial class BasicStationAi : Node
{
	[Export] Sprite2D StationSprite;
	private readonly RandomNumberGenerator _rng = new();

	private Array<BaseWeapon> _currentWeapon;
	private Player _target;
	public override void _Ready()
	{
		_rng.Randomize();
		StationSprite.Rotation = _rng.Randf();
	}

	
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		StationSprite.Rotation = StationSprite.Rotation + 0.0001f;
	}

	private void AcquireTarget()
	{
		Node scene = GetTree().CurrentScene;
		_target = scene.GetNode<Player>("Player");
	}

	private void FireWeapons(float distance)
	{
		
	}

	/*protected virtual void SelectWeapon()
	{
		_currentWeapon = Primary;
		
		switch (_rng.RandiRange(0, 2))
		{
			case 0:
				_currentWeapon = Primary;
				break;
			case 1:
				_currentWeapon = Secondary;
				break;
			case 2:
				_currentWeapon = Tertiary;
				break;
		}
	}*/
}
