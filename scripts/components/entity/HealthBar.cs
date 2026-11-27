using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MagicFarmer.scripts.ui;

namespace MagicFarmer.scripts.components.entity;

[GlobalClass]
[Tool]
public partial class HealthBar : Control
{
	private HealthPool _healthPool;

	[Export]
	public HealthPool HealthPool
	{
		get => _healthPool;
		set
		{
			_healthPool = value;
			UpdateConfigurationWarnings();
		}
	}

	public Func<int, int, Task> UpdateHealthFunction { get; set; }

	public override string[] _GetConfigurationWarnings()
	{
		if (GetTree()?.EditedSceneRoot == this)
		{
			return [];
		}
		var warnings = new List<string>();

		var baseWarnings = base._GetConfigurationWarnings();

		if (baseWarnings is { Length: > 0 })
		{
			warnings.AddRange(baseWarnings);
		}

		if (HealthPool == null)
		{
			warnings.Add("HealthBar is null");
		}
		
		if (warnings.Count > 0 && Engine.IsEditorHint())
		{
			GD.PrintErr($"{GetParent()?.Name} has configuration warnings with {Name}");
		}
		
		return warnings.ToArray();
	}

	private void UpdateHealthBar(int currentHealth, int baseHealth)
	{
		
		UpdateHealthFunction(currentHealth, baseHealth);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		HealthPool.HealthChanged += UpdateHealthBar;
	}
}
