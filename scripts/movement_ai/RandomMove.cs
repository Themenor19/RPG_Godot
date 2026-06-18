using System.Collections.Generic;
using Godot;

namespace RPG.scripts.movement_ai;


public partial class RandomMove: Node2D
{
	private enum RandomMoveActions
	{
		Idle,
		NewDir,
		Move
	}
	[Export] 
	public AnimatedSprite2D Sprite;
	[Export]
	public Timer ActionTimer;
	[Export]
	public CharacterBody2D CharacterBody;
	[Export]
	public float Speed = 100f;
	
	public bool CanMove = true;
	
	private RandomMoveActions _currentState = RandomMoveActions.Idle;
	private Vector2 _dir = Vector2.Right;
	private Vector2 _startPosition;
	private bool _isRoaming = true;
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = CharacterBody.Velocity;

		if (_currentState is RandomMoveActions.Idle or RandomMoveActions.NewDir)
		{
			velocity.X = Mathf.MoveToward(CharacterBody.Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(CharacterBody.Velocity.Y, 0, Speed);

			if (_dir == Vector2.Down)
			{
				Sprite.Play("front_standing_idle");
			}
			else if (_dir == Vector2.Up)
			{
				Sprite.Play("back_standing_idle");
			}
			else if (_dir == Vector2.Left)
			{
				Sprite.Play("left_standing_idle");
			}
			else if (_dir == Vector2.Right)
			{
				Sprite.Play("right_standing_idle");
			} 
			else
			{
				Sprite.Play("front_standing_idle");
			}
			
		}
		else if (_currentState is RandomMoveActions.Move)
		{
			velocity = _dir*Speed;
			if (Mathf.Abs(_dir.X) >= Mathf.Abs(_dir.Y))
			{
				if (_dir.X < 0) Sprite.Play("walk_w");
				else if (_dir.X > 0) Sprite.Play("walk_e");
			}
			else
			{
				if (_dir.Y < 0) Sprite.Play("walk_n");
				else if (_dir.Y > 0) Sprite.Play("walk_s");
			}
		}

		if (_isRoaming)
		{
			CharacterBody.Velocity = velocity;
			switch (_currentState)
			{
				case RandomMoveActions.Idle:
					break;
				case RandomMoveActions.NewDir:
					_dir = Choose([ Vector2.Up, Vector2.Left, Vector2.Down, Vector2.Right ]);
					_currentState = RandomMoveActions.Idle;
					break;
				case RandomMoveActions.Move:
					Move();
					break;
			}
		}
	}
	
	private void Move()
	{
		if (!CanMove)
		{
			CharacterBody.MoveAndSlide();
		}
	}
	

	private dynamic Choose(List<dynamic> list)
	{ 
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = (int)(GD.Randi() % (uint)(i + 1));
			(list[i], list[j]) = (list[j], list[i]);
		}
		return list[0];
	}
}
