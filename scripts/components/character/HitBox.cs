using System.Collections.Generic;
using Godot;

namespace RPG.scripts.character_components;

[Tool]
public partial class HitBox : Area2D
{
	private MagicFarmer.scripts.ui.HealthPool _healthPool;

	[Export]
	public MagicFarmer.scripts.ui.HealthPool HealthPool
	{
		get => _healthPool;
		set
		{
			_healthPool = value;
			UpdateConfigurationWarnings();
		}
	}
	
	public CollisionShape2D HitBoxShape;

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();
		
		var baseWarnings = base._GetConfigurationWarnings();
		if (baseWarnings is { Length: > 0 })
		{
			warnings.AddRange(baseWarnings);
		}

		if (_healthPool == null)
		{
			warnings.Add("Health Bar is NULL. Consider adding a HealthPool component.");
		}
		
		return warnings.ToArray();
	}

	public void AddCurrentHealth(int amount)
	{
		HealthPool.AddCurrentHealth(amount);
	}

	public override void _Ready()
	{
		HitBoxShape = GetChild<CollisionShape2D>(0);
	}
}
