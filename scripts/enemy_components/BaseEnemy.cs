using System.Collections.Generic;
using Godot;

namespace RPG.scripts.enemy_components;

[Tool]
[GlobalClass]
public partial class BaseEnemy : CharacterBody2D
{
	private character_components.HitBox _enemyHitbox;
	private HealthBar _enemyHealthBar;
	
	[Export]
	public character_components.HitBox EnemyHitbox
	{
		get => _enemyHitbox;
		set
		{
			_enemyHitbox = value;
			UpdateConfigurationWarnings();
		}
	}

	[Export]
	public HealthBar EnemyHealthBar
	{
		get => _enemyHealthBar;
		set
		{
			_enemyHealthBar = value;
			UpdateConfigurationWarnings();
		}
	}

	[Export] public int StartingHealth = 50;

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();
		
		var baseWarnings = base._GetConfigurationWarnings();
		if (baseWarnings is { Length: > 0 })
		{
			warnings.AddRange(baseWarnings);
		}
		
		if (_enemyHealthBar == null)
		{
			warnings.Add("Health Bar is NULL. Consider adding a health bar component.");
		}

		if (_enemyHitbox == null)
		{
			warnings.Add("Hitbox is NULL. Consider adding a hitbox component.");
		}

		return warnings.ToArray();
	}

	public override void _Ready()
	{
		EnemyHealthBar.SetHealthBar(StartingHealth);
	}
	
	
	public override void _PhysicsProcess(double delta)
	{
		/*Vector2 velocity = Velocity;

	// Add the gravity.
	if (!IsOnFloor())
	{
		velocity += GetGravity() * (float)delta;
	}

	// Handle Jump.
	if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
	{
		velocity.Y = JumpVelocity;
	}

	// Get the input direction and handle the movement/deceleration.
	// As good practice, you should replace UI actions with custom gameplay actions.
	Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
	if (direction != Vector2.Zero)
	{
		velocity.X = direction.X * Speed;
	}
	else
	{
		velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
	}

	Velocity = velocity;
	MoveAndSlide();*/
	}
}
