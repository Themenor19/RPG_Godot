using Godot;
using System;
using RPG.scripts.helper_classes;

public partial class DayNightCycleOverlay : Control
{
	[Export] private ColorRect _overlay;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DayNightCycle.Instance.ColorChanged += OnColorChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.

	private void OnColorChanged(Color color)
	{
		_overlay.Modulate = color;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		DayNightCycle.Instance.ColorChanged -= OnColorChanged;
	}
}
