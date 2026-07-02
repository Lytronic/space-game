using Godot;
using Microgravity.util;
using System.Threading.Tasks;

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

	// Speed Variables
	private float _angularVelocity = 0.0f;
	private float _throttleSpeed = 0.0f;
	private float _strafeSpeed = 0.0f;

	// User Interfaces
	private Control _hud;
	private Control _deathScreen;
	private ColorRect _damageOverlay;

	// Labels (not the queer kind.. probably...)
	private Label _playerSpeedLabel;
	private Label _playerHealthLabel;
	private Label _playerScoreLabel;

	// Healthy variables
	public bool IsAlive = true;
	
	// Cursor Variables
	private Sprite2D _cursorThrottle;

	public override void _Ready()
	{
		// Assign user interfaces
		_hud = GetNode<Control>("../CanvasLayer/HUD");
		_deathScreen = GetNode<Control>("../CanvasLayer/DeathScreen");
		_damageOverlay = GetNode<ColorRect>("../CanvasLayer/DamageOverlay");

		// Assign labels
		_playerSpeedLabel = _hud.GetNode<Label>("PlayerSpeedLabel");
		_playerHealthLabel = _hud.GetNode<Label>("PlayerHealthLabel");
		_playerScoreLabel = _hud.GetNode<Label>("PlayerScoreLabel");

		// You get the point
		_hud.Show();
		_deathScreen.Hide();
		
		// Cursor setup
		_cursorThrottle = GetNode<Sprite2D>("/root/game/Cursor/CursorThrottle");

	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		if (IsAlive)
		{
			// Ship movement & behavior
			RotateTowardMouse(dt);
			// MoveAndSlide();
			var collisionInfo = MoveAndCollide(Velocity * dt);
			if (collisionInfo != null)
			{
				Vector2 deltaV = Velocity.Length() > 0.0f ? Velocity - collisionInfo.GetColliderVelocity() : new Vector2(0.0f, 0.0f);

				Velocity = new Vector2(0.0f, 0.0f);
				_throttleSpeed = 0.0f;
				_strafeSpeed = 0.0f;
				
				GD.Print(Velocity);
				GD.Print($"{deltaV}, absolute: {deltaV.Length()}");				

				PlayerVariables.Instance.CurrentHealth -= Mathf.Pow(deltaV.Length() * 0.01f, 2.0f);
			} else {
				UpdateLinearMovement(dt);
			}

			// Check for player death
			if (PlayerVariables.Instance.CurrentHealth <= 0.0001f)
			{
				IsAlive = false;
				InitiateDeathSequence();
			}
		}

	}

	public override void _Process(double delta)
	{
		// Update speed value on HUD
		_playerSpeedLabel.Text = $"Speed: {Velocity.Length():0}";

		// Update health value on HUD
		_playerHealthLabel.Text = $"Health: {PlayerVariables.Instance.CurrentHealth:0}";

		_playerScoreLabel.Text = PlayerVariables.Instance.Score.ToString();

		// update damage overlay (< 25 health only)
		((ShaderMaterial)_damageOverlay.Material).SetShaderParameter("intensity", PlayerVariables.Instance.CurrentHealth <= 25.0f ?
			((1.0f - PlayerVariables.Instance.CurrentHealth / 100.0f) * (((SettingsEntry.Float)SettingsModel.Instance.Settings["video.damage_overlay_intensity"]).Value / 100.0f))
			: 0.0f);
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
		PlayerVariables.Instance.CurrentHealth -= amount;
	}

	private async void InitiateDeathSequence()
	{
		// Disable HUD
		_hud.Hide();

		// Wait briefly
		await ToSignal(GetTree().CreateTimer(2.5), SceneTreeTimer.SignalName.Timeout);

		// Display game over screen
		_deathScreen.Show();
		
		// Enable Cursor again
		Input.SetMouseMode(Input.MouseModeEnum.Visible);
	}
}
