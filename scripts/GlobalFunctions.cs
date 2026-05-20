using System;
using Godot;

namespace RPG.scripts;

public partial class GlobalFunctions : Node
{
	private static readonly Vector2 BaseSize = new(480f, 270.0f);
	public static GlobalFunctions Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateSize();
		GetTree().GetRoot().SizeChanged += UpdateSize;
	}

	public void UpdateSize()
	{
		Vector2 sz = DisplayServer.WindowGetSize();
		float ratio = Math.Min(sz.X/BaseSize.X, sz.Y/BaseSize.Y);
		ratio = (float)Math.Max(1f, Math.Floor(ratio));
		GetWindow().ContentScaleFactor = ratio;
	}
	
}
