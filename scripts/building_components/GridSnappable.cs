using Godot;

namespace RPG.scripts.building_components;

public partial class GridSnappable : Node2D
{
	[Export] public TileMapLayer GridLayer;
	[Export] public Node2D SnappableNode;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetNotifyTransform(true);
	}


	public override void _Notification(int what)
	{
		base._Notification(what);

		if (what == NotificationTransformChanged && Engine.IsEditorHint())
		{
			SnapNodeToGrid();
		}
	}

	public void SnapNodeToGrid()
	{
		if (GridLayer == null) return;
		
		var localNodeCoordinates = GridLayer.ToLocal(SnappableNode.GlobalPosition);
		var snapCoords = GridLayer.MapToLocal(GridLayer.LocalToMap(localNodeCoordinates));
		SnappableNode.GlobalPosition = GridLayer.ToGlobal(snapCoords);
	}
}
