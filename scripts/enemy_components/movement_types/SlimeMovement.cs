using System;
using System.Collections.Generic;
using Godot;

namespace RPG.scripts.enemy_components.movement_types;

public partial class SlimeMovement : Node2D
{
	[Signal]
	public delegate void CurrentAnimationFinishedEventHandler();
	[Export] public bool Hopping;
	[Export] public Timer ActionTimer;
	[Export] public CharacterBody2D Character;
	[Export] public AnimationPlayer Sprite;
	[Export] public float Speed = 100f;
	private enum ActionType
	{
		Idle,
		NewDir,
		Hop,
		Attack
	}
	
	private ActionType _currentState = ActionType.Idle;
	
	private Vector2 _dir = Vector2.Right;

	private bool _playerInRange;
	private bool _animationFinished = true;

	private string _currentAnimation = "";


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Character.Velocity;

		if (_currentState is ActionType.Idle or ActionType.NewDir)
		{
			velocity.X = Mathf.MoveToward(Character.Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Character.Velocity.Y, 0, Speed);


			Sprite.Play("idle_blinking");


		}
		else if (_currentState is ActionType.Hop)
		{
			velocity = _dir * Speed;
			if (Mathf.Abs(_dir.X) >= Mathf.Abs(_dir.Y))
			{
				if (_dir.X < 0)
				{
					Sprite.Play("move_west");
					_currentAnimation = "move_west";
				}
				else if (_dir.X > 0)
				{
					Sprite.Play("move_east");
					_currentAnimation = "move_east";
				}
			}
			else
			{
				if (_dir.Y < 0)
				{
					Sprite.Play("move_north");
					_currentAnimation = "move_north";
				}
				else if (_dir.Y > 0)
				{
					Sprite.Play("move_south");
					_currentAnimation = "move_south";
				}
			}

			_animationFinished = false;
		}

		Character.Velocity = velocity;
		switch (_currentState)
		{
			case ActionType.Idle:
				break;
			case ActionType.NewDir:
				_dir = Choose([Vector2.Up, Vector2.Left, Vector2.Down, Vector2.Right]);
				_currentState = ActionType.Hop;
				break;
			case ActionType.Hop:
				Move();
				break;
		}
	}

	private void Move()
	{
		if (Hopping)
		{
			Character.MoveAndSlide();
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

	public void _on_timer_timeout()
	{
		if (!_animationFinished)
		{
			CurrentAnimationFinished += _on_timer_timeout;
			return;
		}
		else
		{
			CurrentAnimationFinished -= _on_timer_timeout;
		}

		if (_playerInRange)
		{
			_currentState = ActionType.Attack;
		}
		else
		{
			_currentState = Choose([ActionType.Idle, ActionType.NewDir]);
			ActionTimer.WaitTime = Choose([1, 1.5, 2]);
		}
	}

	private void _on_animation_finished(StringName animationName)
	{
		if (animationName == _currentAnimation)
		{
			_animationFinished = true;
			EmitSignal(SignalName.CurrentAnimationFinished);
		}
	}
	
	public override void _EnterTree()
	{
		ActionTimer.Timeout += _on_timer_timeout;
		Sprite.AnimationFinished += _on_animation_finished;
	}

	public override void _ExitTree()
	{
		ActionTimer.Timeout -= _on_timer_timeout;
		Sprite.AnimationFinished -= _on_animation_finished;
	}
}
