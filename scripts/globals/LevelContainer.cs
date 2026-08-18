using Godot;
using System;

public partial class LevelContainer : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Center();
		GetTree().Root.SizeChanged += Center;
	}

	private void Center()
	{
		Position = GetViewport().GetVisibleRect().Size / 2;
	}
}
