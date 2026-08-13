using System;
using System.Collections.Generic;
using Godot;

namespace RPG.scripts.enemy_components.movement_types;

public partial class SlimeMovement : Node2D
{
	private Timer _actionTimer;
	private CharacterBody2D _character;
	private AnimationPlayer _sprite;
	private DetectionArea _detectionArea;
	private float _speed = 100f;
	
	[Signal]
	public delegate void CurrentAnimationFinishedEventHandler();
	[Export] public bool Hopping;

	[Export]
	public Timer ActionTimer
	{
		get => _actionTimer;
		set
		{
			_actionTimer = value;
			UpdateConfigurationWarnings();
		}
	}
	[Export]
	public CharacterBody2D Character
	{
		get => _character;
		set
		{
			_character = value;
			UpdateConfigurationWarnings();
		}
	}
	[Export]
	public AnimationPlayer Sprite
	{
		get => _sprite;
		set
		{
			_sprite = value;
		}
	}
	[Export]
	public DetectionArea DetectionArea
	{
		get => _detectionArea;
		set
		{
			_detectionArea = value;
			UpdateConfigurationWarnings();
		}
	}
	[Export]
	public float Speed
	{
		get => _speed;
		set
		{
			if (value <= 0)
			{
				value = 0;
			}
			_speed = value;
			UpdateConfigurationWarnings();
		}
	}
	
	
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
	private Player _player;
	private bool _animationFinished = true;

	private string _currentAnimation = "";

	public override string[] _GetConfigurationWarnings()
	{
		List<string> warnings = [];
		
		var baseWarnings = base._GetConfigurationWarnings();
		if (baseWarnings != null && baseWarnings.Length > 0)
		{
			warnings.AddRange(baseWarnings);
		}

		if (ActionTimer == null)
		{
			warnings.Add("Action Timer is NULL. Consider adding a timer to allow actions to fire");
		}
		if (Character == null)
		{
			warnings.Add("Character is NULL. Consider adding a CharacterBody2D");
		}
		if (DetectionArea == null)
		{
			warnings.Add(
				"DetectionArea is NULL. Consider adding a DetectionArea to allow the character to detect objects");
		}
		if (Sprite == null)
		{
			warnings.Add("Sprite is NULL. Consider adding an AnimationPlayer2D");
		}
		
		return warnings.ToArray();
	}

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
			case ActionType.Attack:
				Attack();
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

	private void Attack()
	{
		
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

		CurrentAnimationFinished -= _on_timer_timeout;

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

	private void _on_body_entered(Node body)
	{
		if (body is Player player)
		{
			_playerInRange = true;
			_player = player;
		}
	}

	private void _on_body_exited(Node body)
	{
		if (body is Player)
		{
			_playerInRange = false;
			_player = null;
		}
	}
	
	public override void _EnterTree()
	{
		ActionTimer.Timeout += _on_timer_timeout;
		Sprite.AnimationFinished += _on_animation_finished;
		DetectionArea.BodyEntered += _on_body_entered;
		DetectionArea.BodyExited += _on_body_exited;
	}

	public override void _ExitTree()
	{
		ActionTimer.Timeout -= _on_timer_timeout;
		Sprite.AnimationFinished -= _on_animation_finished;
		DetectionArea.BodyEntered -= _on_body_entered;
		DetectionArea.BodyExited -= _on_body_exited;
	}
}
