using System.Collections.Generic;
using Godot;
using MagicFarmer.scripts.ui;

namespace MagicFarmer.scripts.components.enemy;

[Tool]
[GlobalClass]
public partial class BaseEnemy : CharacterBody2D
{
	private RPG.scripts.character_components.HitBox _enemyHitbox;
	private HealthPool _enemyHealthPool;
	
	[Export]
	public RPG.scripts.character_components.HitBox EnemyHitbox
	{
		get => _enemyHitbox;
		set
		{
			_enemyHitbox = value;
			UpdateConfigurationWarnings();
		}
	}

	[Export]
	public HealthPool EnemyHealthPool
	{
		get => _enemyHealthPool;
		set
		{
			_enemyHealthPool = value;
			UpdateConfigurationWarnings();
		}
	}

	[Export] public int StartingHealth = 50;
	[Export] public int TouchDamage;

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();
		
		var baseWarnings = base._GetConfigurationWarnings();
		if (baseWarnings is { Length: > 0 })
		{
			warnings.AddRange(baseWarnings);
		}
		
		if (_enemyHealthPool == null)
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
		EnemyHealthPool.SetHealthBar(StartingHealth);
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
