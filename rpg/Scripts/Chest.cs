using Godot;
using System;

public partial class Chest : Area2D
{
	
	[Export]
	private Vector2 _startingPosition = Vector2.Zero;
	[Signal] public delegate void ChestOpenedEventHandler();
	[Signal] public delegate void ChestClosedEventHandler();
	
	public Chest()
	{
		
	}

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
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
