using Godot;
using System;
using Godot.Collections;

[Tool]
public partial class Building : StaticBody2D
{
	private bool _snapNodeToGrid;

	[Export]
	public bool SnapNodeToGrid
	{
		get => _snapNodeToGrid;
		set
		{
			_snapNodeToGrid = value;
			NotifyPropertyListChanged();
		}
	}
	[Export] public TileMapLayer GridLayer;

	public override void _ValidateProperty(Dictionary property)
	{
		base._ValidateProperty(property);
		
		var propertyName = property["name"].AsString();
		if (propertyName == PropertyName.GridLayer)
		{
			if (!SnapNodeToGrid)
			{
				property["usage"] = (int)PropertyUsageFlags.NoEditor;
			}
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetNotifyTransform(true);
	}


	public override void _Notification(int what)
	{
		base._Notification(what);

		if (what == NotificationTransformChanged && Engine.IsEditorHint() && SnapNodeToGrid)
		{
			SnapToGrid();
		}
	}

	public void SnapToGrid()
	{
		if (GridLayer == null) return;
		
		var localDoorCoords = GridLayer.ToLocal(GlobalPosition);
		var snapCoords = GridLayer.MapToLocal(GridLayer.LocalToMap(localDoorCoords));
		GlobalPosition = GridLayer.ToGlobal(snapCoords);
	}
}
