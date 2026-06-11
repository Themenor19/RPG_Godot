using Godot;
using System;

public partial class UsagePanel : NinePatchRect
{
	private bool entered;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Visible) return;
		var mousePos = GetViewport().GetMousePosition();
		var rect = new Rect2(GlobalPosition, Size);
		if (entered && !rect.HasPoint(mousePos))
		{
			Visible = false;
			entered = false;
		}
		else if (!entered && rect.HasPoint(mousePos))
		{
			entered = true;
		}
	}
	
	private void _on_mouse_exited()
	{
		var mousePos = GetViewport().GetMousePosition();
		var rect = new Rect2(GlobalPosition, Size);
	
		if (!rect.HasPoint(mousePos))
		{
			Visible = false;
		}
	}
}
