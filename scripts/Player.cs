using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 100.0f;
	public const float JumpVelocity = -400.0f;
	public AnimatedSprite2D sprite;
	public char Direction = 'd';

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		sprite.Play("front_standing_idle");
		Godot.GD.Print(sprite.Animation);
		
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{

			if (Math.Abs(direction.X) > Math.Abs(direction.Y))
			{
				bool goingRight = direction.X > 0;
				sprite.Play(goingRight? "walk_right" : "walk_left");
				Direction = goingRight ? 'r' : 'l';
			}
			else
			{
				bool goingUp = direction.Y < 0;
				sprite.Play(goingUp ? "walk_up" : "walk_down");
				Direction = goingUp ? 'u' : 'd';
			}
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);

			switch (Direction)
			{
				case 'd':
					sprite.Play("front_standing_idle");
					break;
				case 'u':
					sprite.Play("back_standing_idle");
					break;
				case 'l':
					sprite.Play("left_standing_idle");
					break;
				case 'r':
					sprite.Play("right_standing_idle");
					break;
				default:
					sprite.Play("front_standing_idle");
					break;
			}
			
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
