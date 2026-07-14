using Godot;
using Microgravity.util;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;

public partial class Player : CharacterBody2D
{
	// Throttle variables
	[Export] public float MaxThrottleSpeed = 250.0f;
	[Export] public float ThrottleAcceleration = 300.0f;

	// Strafing variables
	[Export] public float MaxStrafeSpeed = 180.0f;
	[Export] public float StrafeAcceleration = 300.0f;

	// Angular movement variabels
	[Export] public float AngularAcceleration = 8.0f;
	[Export] public float MaxAngularSpeed = 6.0f;
	[Export] public float AngularStopEpsilon = 0.01f;

	[Export] public BaseWeapon Weapon;

	// Speed Variables
	private float _angularVelocity = 0.0f;
	private float _throttleSpeed = 0.0f;
	private float _strafeSpeed = 0.0f;

	// Particles variables
	[Export] public float ParticleMinThrust = 10.0f;
	private GpuParticles2D _engineParticles0;
	private GpuParticles2D _engineParticles1;
	private GpuParticles2D _engineParticles2;
	private GpuParticles2D _explosionParticle;
	private Sprite2D _playerShip;
	

	// User Interfaces
	private Control _hud;
	private Control _deathScreen;
	private ColorRect _damageOverlay;
	private ShaderMaterial _damageOverlayMaterial;
	private TextureRect _enemyLocator;

	// Labels (not the queer kind.. probably...)
	private Label _playerScoreLabel;
	private Label _roundLabel;

	// Progress bars
	private ProgressBar _playerSpeedBar;
	private ProgressBar _playerHealthBar;
	private ProgressBar _playerShieldBar;
	private ProgressBar _playerEnergyBar;
	private ProgressBar _playerAmmoBar;
	private ProgressBar _playerFuelBar;


	// Healthy variables
	public bool IsAlive = true;

	// Cooldown timers
	private SceneTreeTimer _damageTintCooldown;
	private SceneTreeTimer _stunTimer;
	
	// Cursor Variables
	private Sprite2D _cursorThrottle;

	// Sound
	private SoundManager _soundManager;

	// alternative textures
	private Texture2D _texture1;
	private Texture2D _texture2;
	private Texture2D _texture3;

	// the parts manager holds on to the weapon nodes
	private PartsManager partsManager;
	public override void _Ready()
	{
		// Assign user interfaces
		_hud = GetNode<Control>("../CanvasLayer/HUD");
		_deathScreen = GetNode<Control>("../CanvasLayer/DeathScreen");
		_damageOverlay = GetNode<ColorRect>("../CanvasLayer/DamageOverlay");
		_damageOverlayMaterial = _damageOverlay.Material as ShaderMaterial;
		_enemyLocator = _hud.GetNode<TextureRect>("EnemyLocator");
		
		// Assign labels
		_playerScoreLabel = _hud.GetNode<Label>("PlayerScoreLabel");
		_roundLabel = _hud.GetNode<Label>("RoundIndicator/RoundLabel");

		// Assign Bars
		_playerSpeedBar = _hud.GetNode<ProgressBar>("SpeedBar");
		_playerHealthBar = _hud.GetNode<ProgressBar>("HealthBar");
		_playerShieldBar = _hud.GetNode<ProgressBar>("ShieldBar");
		_playerEnergyBar = _hud.GetNode<ProgressBar>("EnergyBar");
        _playerAmmoBar = _hud.GetNode<ProgressBar>("AmmoBar");
        _playerFuelBar = _hud.GetNode<ProgressBar>("FuelBar");

		// You get the point
		_hud.Show();
		_deathScreen.Hide();
		
		// Cursor setup
		_cursorThrottle = GetNode<Sprite2D>("/root/game/Cursor/CursorThrottle");

		// Particles setup
		_explosionParticle = GetNode<GpuParticles2D>("ExplosionParticle");
		GD.Print(_explosionParticle);
		_engineParticles0 = GetNode<GpuParticles2D>("EngineParticles0");
		_engineParticles1 = GetNode<GpuParticles2D>("EngineParticles1");
		_engineParticles2 = GetNode<GpuParticles2D>("EngineParticles2");
		_playerShip = GetChild(3) as Sprite2D;
		GD.Print(_playerShip);

		_explosionParticle.Emitting = false;

		_soundManager = GetNode<SoundManager>("/root/SoundManager");

		// adding this as an action so we can check when it is released, too
		if (!InputMap.HasAction("fire"))
		{
			InputMap.AddAction("fire");
			InputMap.ActionAddEvent("fire", new InputEventMouseButton() { ButtonIndex = MouseButton.Left });
		}

		// initialise timers
		_damageTintCooldown = GetTree().CreateTimer(0.0f);
		_stunTimer = GetTree().CreateTimer(0.0f);

        //alternative ship textures loaded
        _texture1  = GD.Load<Texture2D>("res://gfx/game/ship.png") ;
        _texture2 = GD.Load<Texture2D>("res://gfx/game/ship2.png") ;
        _texture3 = GD.Load<Texture2D>("res://gfx/game/ship3.png") ;

        //Parts Manager
        partsManager = GetChildren().OfType<PartsManager>().First();
		
    }

    public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		if (IsAlive)
		{
			// Ship movement & behavior
			RotateTowardMouse(dt);

			var collisionInfo = MoveAndCollide(Velocity * dt);
			if (collisionInfo != null)
			{
				Vector2 deltaV = Velocity.Length() > 0.0f ? Velocity - collisionInfo.GetColliderVelocity() : new Vector2(0.0f, 0.0f);

				Velocity = new Vector2(0.0f, 0.0f);
				_throttleSpeed = 0.0f;
				_strafeSpeed = 0.0f;
				
				TakeDamage(Mathf.Pow(deltaV.Length() * 0.01f, 2.0f));
			} else {
				UpdateLinearMovement(dt);
			}

			// Check for player death
			if (PlayerVariables.Stats.CurrentHealth <= 0.0001f)
			{
				IsAlive = false;
				InitiateDeathSequence();
			}
			
		}

	}

	public override void _Process(double delta)
	{
		// Update speed value on HUD
		_playerHealthBar.Value = PlayerVariables.Stats.CurrentHealth;

		// Update health value on HUD
		_playerSpeedBar.Value = Velocity.Length();


		_playerEnergyBar.Value = PlayerVariables.Stats.Energy;
        _playerAmmoBar.Value = PlayerVariables.Stats.Ammo;
        _playerFuelBar.Value = PlayerVariables.Stats.Fuel;

		_playerShieldBar.Value = PlayerVariables.Stats.CurrentShield;

		_playerScoreLabel.Text = PlayerVariables.Stats.Score.ToString();
		_roundLabel.Text = PlayerVariables.Stats.Round.ToString();

		UpdateDamageOverlay();
		UpdateEnemyLocator();
		UpdateWeapon((float)delta);
		ManageEngineParticles();
		UpdateSprite();
		_soundManager.ChangeFlightNoise(Velocity.Length());
		
	}

	/// <summary>
	/// Update the damage overlay based on which situation the player is currently in.
	/// This sets the intensity uniform to the highest contribution from either the health value or recent incoming damage.
	///
	/// If the player's shield is active, the tint should be blue to indicate its utilisation instead.
	/// </summary>
	private void UpdateDamageOverlay()
	{
		float intensityModifier = ((SettingsEntry.Float)SettingsModel.Instance.Settings["video.damage_overlay_intensity"]).Value / 100.0f;

		float timeLeft = _damageTintCooldown != null ? (float)_damageTintCooldown.TimeLeft : 0.0f; 
		float damageIntensity = (float)Mathf.Clamp(timeLeft * 10.0f, 0.0f, 1.0f);

		float health = PlayerVariables.Stats.CurrentHealth / 100.0f;
		float healthIntensity = health < 0.25f ? 1 - health : 0.0f;

		if (PlayerVariables.Stats.CurrentShield > 0.0f && !(healthIntensity > 0.0f))
		{
			_damageOverlayMaterial.SetShaderParameter("colour", new Vector3(0.0f, 0.0f, 1.0f));
		} else {
			_damageOverlayMaterial.SetShaderParameter("colour", new Vector3(1.0f, 0.0f, 0.0f));
		}

		_damageOverlayMaterial.SetShaderParameter("intensity", Mathf.Max(damageIntensity, healthIntensity) * intensityModifier);
	}

	public void UpdateEnemyLocator()
	{
		var entities = PlayerVariables.Space.GetChildren();

		BaseEnemy nearest = null;

		foreach (var entity in entities)
		{
			if (entity is BaseEnemy enemy)
			{				
				nearest ??= enemy;
				nearest = (enemy.GlobalPosition - GlobalPosition).Length()
					< (nearest.GlobalPosition - GlobalPosition).Length() && !enemy.IsDead ? enemy : nearest;
			}
		}

		Vector2 toNearest = (nearest.GlobalPosition - GlobalPosition).Normalized();

		// float sign = Mathf.Sign(toNearest.Y);
		_enemyLocator.Rotation = toNearest.Angle();
	}

	// Now rotates to the CursorThrottle instead of the mouse
	private void RotateTowardMouse(float dt)
	{

		Vector2 mouseVec = _cursorThrottle.GlobalPosition - GlobalPosition;

		if (_cursorThrottle == null) {
			mouseVec = GetGlobalMousePosition() - GlobalPosition;
		}

		if (mouseVec.LengthSquared() < 0.0001f) return; // Floating point correction

		float targetRotation = mouseVec.Angle() + Mathf.Pi / 2.0f; // Pi/2 = 90°, added to target rot to calc up vec
		float angleDiff = Mathf.Wrap(targetRotation - Rotation, -Mathf.Pi, Mathf.Pi); // Get angle diff and clamp it between +-180°

		// Stop ang mot if ang vel under thresh
		if (Mathf.Abs(angleDiff) < AngularStopEpsilon && Mathf.Abs(_angularVelocity) < 0.05f)
		{
			Rotation = targetRotation;
			_angularVelocity = 0.0f;
			return;
		}

		float direction = Mathf.Sign(angleDiff); // Check if angle diff is + or -
		float stoppingDist = (_angularVelocity * _angularVelocity) / (2.0f * AngularAcceleration); // Since a is const, d = v^2 / 2a (thx google)

		if (Mathf.Abs(angleDiff) <= stoppingDist) // Check if within stopping dist
		{
			_angularVelocity -= direction * AngularAcceleration * dt; // Slow down ang vel by a each frame

			if (Mathf.Sign(_angularVelocity) != direction) _angularVelocity = 0.0f; // Stop ang mot if overshoot (means ang is rougly on target)
		}
		else // (not within stopping dist)
		{
			_angularVelocity += direction * AngularAcceleration * dt; // Accelerate ang vel by a each frame
			_angularVelocity = Mathf.Clamp(_angularVelocity, -MaxAngularSpeed, MaxAngularSpeed); // Clamp ang vel between +-max ang vel
		}

		float step = _angularVelocity * dt; // Calc how far ang move this frame

		if (Mathf.Abs(step) > Mathf.Abs(angleDiff)) // Check if we reaching target rot this frame
		{
			Rotation = targetRotation;
			_angularVelocity = 0.0f;
		}
		else Rotation += step; // Apply ang move
	}

	private void UpdateLinearMovement(float dt)
	{
		if (_stunTimer.TimeLeft > 0) return;
		
		float verticalInput = Input.GetAxis("backward", "forward");
		float horizontalInput = Input.GetAxis("left", "right");

		_throttleSpeed = UpdateAxisSpeed(
			_throttleSpeed,
			verticalInput,
			MaxThrottleSpeed,
			ThrottleAcceleration,
			dt
		);
		_throttleSpeed = Mathf.Clamp(_throttleSpeed, -MaxThrottleSpeed / 2, MaxThrottleSpeed);

		_strafeSpeed = UpdateAxisSpeed(
			_strafeSpeed,
			horizontalInput,
			MaxStrafeSpeed,
			StrafeAcceleration,
			dt
		);

		Vector2 forward = -Transform.Y;
		Vector2 right = Transform.X;

		Velocity = forward * _throttleSpeed + right * _strafeSpeed;
	}

	private void UpdateWeapon(float dt)
	{
		if (IsAlive && Input.IsActionPressed("fire"))
		{
			TryFireWeapon();
		}
		else if (Input.IsActionJustReleased("fire"))
		{
			Weapon.Release();
		}
	}

	private void TryFireWeapon()
	{
		Vector2 direction = GetGlobalMousePosition() - GlobalPosition;
		if (direction.LengthSquared() < 0.0001f)
			return;

		direction = direction.Normalized();

		Weapon.Fire(direction, PlayerVariables.Stats.DamageBase, PlayerVariables.Stats.DamageModif);
	}

	private static float UpdateAxisSpeed(float currentSpeed, float input, float maxSpeed, float accel, float delta)
	{
		if (Mathf.IsZeroApprox(input)) return MoveToward(currentSpeed, 0.0f, accel * delta); // If no axis input, start slowing down

		float targetSpeed = input * maxSpeed; // Always trying to accel to max

		bool reversing = !Mathf.IsZeroApprox(currentSpeed) && Mathf.Sign(input) != Mathf.Sign(currentSpeed); // Reversing if moving opposite to input

		float actualAccel = reversing ? accel * 2.0f : accel; // Accel doubles when reversing to speed up braking

		return MoveToward(currentSpeed, targetSpeed, actualAccel * delta);
	}

	private static float MoveToward(float current, float target, float maxDelta)
	{
		if (current < target) return Mathf.Min(current + maxDelta, target);
		else if (current > target) return Mathf.Max(current - maxDelta, target);
		else return target;
	}

	public void TakeDamage(float amount)
	{
		_damageTintCooldown = GetTree().CreateTimer(Mathf.Clamp(amount / 100.0f, 0.25f, 2.0f));
		
		PlayerVariables.Instance.ApplyDamage(amount);
	}

	/// <summary>
	/// Revoke the player's ability to move for the given duration.
	/// </summary>
	public void Stun(float duration)
	{
		_stunTimer = GetTree().CreateTimer(duration);
	}

	private async void InitiateDeathSequence()
	{
		// Play the explosion particle animation and hide the ship
		Explode();
		
		// Disable HUD
		_hud.Hide();


		// Wait briefly
		await ToSignal(GetTree().CreateTimer(2.5), SceneTreeTimer.SignalName.Timeout);

		// Display game over screen
		_deathScreen.Show();
		
		// Enable Cursor again
		Input.SetMouseMode(Input.MouseModeEnum.Visible);
	}
	
	// Function for the engine particles
	private void ManageEngineParticles()
	{
		float currentSpeed = Velocity.Length();
		/*bool isMoving = Velocity.Length() > ParticleMinThrust;
		_engineParticles0.Emitting = isMoving;
		_engineParticles1.Emitting = isMoving;
		_engineParticles2.Emitting = isMoving;*/
		float maxSpeed = MaxThrottleSpeed;
		float speedRatio = Mathf.Clamp(currentSpeed / maxSpeed, 0.0f, 1.0f);
		if (currentSpeed < 5.0f) 
		{
			_engineParticles0.Emitting = false;
			_engineParticles1.Emitting = false;
			_engineParticles2.Emitting = false;
	   		return;
		}
		_engineParticles0.Emitting = true;
		_engineParticles1.Emitting = true;
		_engineParticles2.Emitting = true;

		int targetAmount = (int)(100 * speedRatio);
		_engineParticles0.Amount = Mathf.Max(targetAmount, 1);
		_engineParticles1.Amount = Mathf.Max(targetAmount, 1);
		_engineParticles2.Amount = Mathf.Max(targetAmount, 1);

		float baseScale = 0.25f;
		
		if (_engineParticles0.ProcessMaterial is ParticleProcessMaterial material)
		{
			float targetScale = baseScale * speedRatio;
			material.SetParamMin(ParticleProcessMaterial.Parameter.Scale, targetScale);
			material.SetParamMax(ParticleProcessMaterial.Parameter.Scale, targetScale);
		}
	}

	private async void Explode()
	{
		foreach (var child in GetChildren())
		{
			if (child.HasMethod(CanvasItem.MethodName.Hide))
			{
				child.Call(CanvasItem.MethodName.Hide);
			}
		}
		_explosionParticle.Show();
		_explosionParticle.OneShot = true;
		_explosionParticle.Restart();
		_explosionParticle.Emitting = true;
		_soundManager.PlaySound(4,7);
		
	}

	public void UpdateSprite()
	{
		float sprite = ((SettingsEntry.Float)SettingsModel.Instance.Settings["general.player_ship"]).Value;

        switch (sprite)
		{
			case 1 :
				_playerShip.Texture = _texture1;

				break;
			case 2 : 
				_playerShip.Texture = _texture2;
				
				break;
			case 3 :
				_playerShip.Texture = _texture3;

				break;
		}
	}

	
}
