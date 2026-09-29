using Godot;
using RPG.scripts.level_scripts;
using Breakable = RPG.scripts.components.building.Breakable;

namespace RPG.scripts;

public partial class Chest : Breakable
{
	[Signal] public delegate void ChestOpenedEventHandler();
	[Signal] public delegate void ChestClosedEventHandler();

	

	public void _on_mouse_entered()
	{
		GD.Print("mouse entered");
		EmitSignal(SignalName.ChestOpened);
	}

	public void _on_mouse_exited()
	{
		GD.Print("mouse exited");
		EmitSignal(SignalName.ChestClosed);
	}
}
