using Godot;
using System;
using RPG.scripts;

public partial class HealthBar : Control
{
	private Label _label; 
	private Global _global;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		_global = Global.Instance;
		_label = GetNode<Label>("MarginContainer/Label");
		SetHealth(); 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SetHealth();
	}

	private void SetHealth()
	{
		if (_global.PlayerNode != null)
		{
			_label.Text = $"{_global.PlayerNode.CurrentHealth}/{_global.PlayerNode.BaseHealth}";
		}
	}
}
