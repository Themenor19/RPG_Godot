using System.Linq;
using Godot;
using RPG.scripts.globals;
using RPG.scripts.helper_classes;

public partial class DayNightCycleOverlay : Control
{
	[Export] private ColorRect _overlay;
	
	private GlobalHandler _global;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		if (_global != null)
		{
			_global.DayNightCycle.ColorChanged += OnColorChanged;
		}
		else
		{
			GD.PrintErr("No DayNightCycle found");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.

	private void OnColorChanged(Color color)
	{
		_overlay.Modulate = color;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_global.DayNightCycle.ColorChanged -= OnColorChanged;
	}
}
