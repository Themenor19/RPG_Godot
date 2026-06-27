using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using RPG.scenes.dialouge;
using RPG.scripts;

namespace RPG.scenes.npcs;

public partial class DefaultNpc : CharacterBody2D
{
	[Export]
	public float Speed = 100f;
	[Export] 
	public HealthBar HealthBar;
	[Export]
	public int StartingHealth = 100;
	private Vector2 _dir = Vector2.Right;
	private Vector2 _startPosition;
	
	private NpcActions _currentState = NpcActions.Idle;

	private bool _isRoaming = true;
	private bool _isChatting;
	private bool _playerInChatZone;
	
	Player _player;
	AnimatedSprite2D _sprite;
	InteractionArea _interactionArea;
	Timer _timer;
	DialoguePlayer _dialogue;
	
	private enum NpcActions
	{
		Idle,
		NewDir,
		Move
	}

	public override void _Ready()
	{
		_startPosition = Position;
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_timer = GetNode<Timer>("Timer");
		_dialogue = GetNode<DialoguePlayer>("DialoguePlayer");
		_interactionArea = GetNode<InteractionArea>("InteractionArea");
		_interactionArea.Interact = () =>
		{
			try
			{
				if (!_isChatting)
				{
					GD.Print("chatting with npc");
					_isRoaming = false;
					_isChatting = true;
					_sprite.Play("idle");
					_dialogue.Start();
				}
				else
				{
					_dialogue.InputPressed();
				}
				return Task.CompletedTask;
			}
			catch (Exception e)
			{
				return Task.FromException(e);
				
			}
		};
		
		HealthBar.SetHealthBar(StartingHealth, StartingHealth);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (_currentState is NpcActions.Idle or NpcActions.NewDir)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);

			if (_dir == Vector2.Down)
			{
				_sprite.Play("front_standing_idle");
			}
			else if (_dir == Vector2.Up)
			{
				_sprite.Play("back_standing_idle");
			}
			else if (_dir == Vector2.Left)
			{
				_sprite.Play("left_standing_idle");
			}
			else if (_dir == Vector2.Right)
			{
				_sprite.Play("right_standing_idle");
			} 
			else
			{
				_sprite.Play("front_standing_idle");
			}
			
		}
		else if (_currentState is NpcActions.Move && !_isChatting)
		{
			velocity = _dir*Speed;
			if (Mathf.Abs(_dir.X) >= Mathf.Abs(_dir.Y))
			{
				if (_dir.X < 0) _sprite.Play("walk_w");
				else if (_dir.X > 0) _sprite.Play("walk_e");
			}
			else
			{
				if (_dir.Y < 0) _sprite.Play("walk_n");
				else if (_dir.Y > 0) _sprite.Play("walk_s");
			}
		}

		if (_isRoaming)
		{
			Velocity = velocity;
			switch (_currentState)
			{
				case NpcActions.Idle:
					break;
				case NpcActions.NewDir:
					_dir = Choose([ Vector2.Up, Vector2.Left, Vector2.Down, Vector2.Right ]);
					_currentState = NpcActions.Idle;
					break;
				case NpcActions.Move:
					Move();
					break;
			}
		}
	}
	
	private void Move()
	{
		if (!_isChatting)
		{
			MoveAndSlide();
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

	public void _on_interaction_area_entered(Node body)
	{
		if (body is Player)
		{
			_player = (Player)body;
			_playerInChatZone = true;
		}
	}

	public void _on_interaction_area_exited(Node body)
	{
		if (body is Player)
		{
			_playerInChatZone = false;
			_isChatting = false;
			_isRoaming = true;
			_currentState = NpcActions.Idle;
			_timer.Stop();
			_timer.WaitTime = 2f;
			_timer.Start();
			_dialogue.Stop();
		}
	}
	
	public void _on_timer_timeout()
	{
		_timer.WaitTime = Choose([0.5, 1, 1.5]);
		_currentState = Choose([NpcActions.NewDir, NpcActions.Move, NpcActions.Idle]);
	}

	public void _on_dialogue_ended()
	{
		_isChatting = false;
		_isRoaming = true;
	}
}
