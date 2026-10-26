using Godot;
using System;
using System.Linq;
using RPG.scripts.globals;

public partial class ControlMenu : Control
{
	[Export] public BaseButton ExitButton;

	private GlobalHandler _global;
	
	private void _on_exit_pressed()
	{
		_global.PopLevelAndMove(transition: false, usePosition: false);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		ExitButton.Pressed += _on_exit_pressed;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		ExitButton.Pressed -= _on_exit_pressed;
	}
}
