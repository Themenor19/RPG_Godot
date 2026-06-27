using System.Collections.Generic;
using Godot;
using RPG.scripts;
using RPG.scripts.character_components;

namespace RPG.scenes.npcs;

public partial class FireBlob : CharacterBody2D
{
	[Export]
	public float Speed = 100f;
	[Export] private int _startingHealth = 100;
	[Export] public HealthBar HealthBar;
	[Export] private HitBox _hitBox;
	[Export] private SpellCaster _spellCaster;
	[Export] private InventoryItem _spellToCast;
	[Export] private float _spellSpeed;
	private Vector2 _dir = Vector2.Right;
	private Vector2 _startPosition;
	
	private NpcActions _currentState = NpcActions.Idle;

	private bool _isRoaming = true;

	
	Player _player;
	[Export]
	public Timer MoveTimer;
	
	private enum NpcActions
	{
		Idle,
		NewDir,
		Move
	}

	public override void _Ready()
	{
		HealthBar.SetHealthBar(_startingHealth, _startingHealth);
		_startPosition = Position;
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (_currentState is NpcActions.Idle or NpcActions.NewDir)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
			
		}
		else if (_currentState is NpcActions.Move )
		{
			velocity = _dir*Speed;
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
		
			MoveAndSlide();
		
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
		MoveTimer.WaitTime = Choose([0.5, 1, 1.5]);
		_currentState = Choose([NpcActions.NewDir, NpcActions.Move, NpcActions.Idle]);
	}

	public void _on_area_2d_body_entered(Node body)
	{
		if (body is Player player)
		{
			_player = player;
		}
	}

	public void _on_area_2d_body_exited(Node body)
	{
		if (body is Player)
		{
			_player = null;
		}
	}

	public void _on_shoot_timer_timeout()
	{
		if (_player != null)
		{
			Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
			Vector2 spawnPosition = GlobalPosition + direction * 10f;
			_spellCaster.CastSpell(_spellToCast, _spellSpeed, _player.GlobalPosition,GetParent(), spawnPosition, _hitBox);
		}
	}

}
