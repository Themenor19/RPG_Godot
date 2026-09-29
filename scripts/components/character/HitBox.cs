using System.Collections.Generic;
using Godot;

namespace RPG.scripts.character_components;

[Tool]
public partial class HitBox : Area2D
{
	private HealthBar _healthBar;

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

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();
		
		var baseWarnings = base._GetConfigurationWarnings();
		if (baseWarnings is { Length: > 0 })
		{
			warnings.AddRange(baseWarnings);
		}

		if (_healthBar == null)
		{
			warnings.Add("Health Bar is NULL. Consider adding a HealthBar component.");
		}
		
		return warnings.ToArray();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddCurrentHealth(int amount)
	{
		HealthBar.AddCurrentHealth(amount);
	}
}
