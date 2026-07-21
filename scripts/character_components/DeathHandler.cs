using System.Collections.Generic;
using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.character_components;

[Tool]
public partial class DeathHandler : Node2D
{
	public enum DeathType {Player, Enemy, Npc}

	//base variables for the exports
	private HealthBar _healthBar;
	private Node2D _parent;
	
	[Export]
	public HealthBar HealthBar
	{
		get => _healthBar;
		set
		{
			_healthBar = value;
			UpdateConfigurationWarnings();
		}
	}

	[Export]
	public Node2D Parent
	{
		get => _parent;
		set
		{
			_parent = value;
			UpdateConfigurationWarnings();
		}
	}
	
	[Export] public DeathType TypeDeath { get; set; } = DeathType.Enemy;

	private Global _global;


	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();
		
		string[] baseWarnings = base._GetConfigurationWarnings();
		if (baseWarnings is { Length: > 0 })
		{
			warnings.AddRange(baseWarnings);
		}

		if (_healthBar == null)
		{
			warnings.Add("Health Bar is NULL. Consider adding a health bar component.");
		}

		if (_parent == null)
		{
			warnings.Add("Parent is NULL. Consider adding a parent component.");
		}
		
		return warnings.ToArray();
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HealthBar.Dead += Die;
		_global = Global.Instance;
	}

	public void Die()
	{
		switch (TypeDeath)
		{
			case DeathType.Player:
				PlayerDeath();
				break;
			case DeathType.Enemy:
				KillEnemy();
				break;
			case DeathType.Npc:
				KillEnemy();
				break;
			default:
				KillEnemy();
				break;
		}
		
	}

	private void PlayerDeath()
	{
		Parent.Visible = false;
		_global.PlayerMoveScenes("uid://bibtx3p5das13");
	}

	private void KillEnemy()
	{
		CallDeferred(nameof(KillParent));
	}
	
	public void KillParent()
	{
		Parent?.QueueFree();
	}
}
