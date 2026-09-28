using Godot;
using System;

[Tool]
public partial class BaseDoor : StaticBody2D
{
	[Export] public bool SnapToGrid = true;
	[Export] public TileMapLayer GridLayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetNotifyTransform(true);
	}


	public override void _Notification(int what)
	{
		base._Notification(what);

		if (what == NotificationTransformChanged && Engine.IsEditorHint() && SnapToGrid)
		{
			SnapDoorToGrid();
		}
	}

	public void SnapDoorToGrid()
	{
		if (GridLayer == null) return;
		
		var localDoorCoords = GridLayer.ToLocal(GlobalPosition);
		var snapCoords = GridLayer.MapToLocal(GridLayer.LocalToMap(localDoorCoords));
		GlobalPosition = GridLayer.ToGlobal(snapCoords);
	}
}
