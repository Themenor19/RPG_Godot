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
	
	private TileMapLayer _mainGridLayer;

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
		if (!Engine.IsEditorHint())
		{
			GetMainGrid();
		}
	}

	private void GetMainGrid()
	{
		Node2D node = this;
		
		while (node != null && node.GetParent() != null)
		{
			var parent = node.GetParent();
			if (parent is not Level level)
			{
				node = parent as Node2D;
				continue;
			}
			_mainGridLayer = level.GroundLayer;
			break;
		}

		if (_mainGridLayer == null)
		{
			GD.PrintErr("Main grid layer not found");
		}
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
		TileMapLayer gridLayer = null;
		if (GridLayer != null) gridLayer = GridLayer;
		if (_mainGridLayer != null) gridLayer = _mainGridLayer;
		if (gridLayer == null) return;
		
		var localDoorCoords = gridLayer.ToLocal(GlobalPosition);
		var snapCoords = gridLayer.MapToLocal(gridLayer.LocalToMap(localDoorCoords));
		GlobalPosition = gridLayer.ToGlobal(snapCoords);
	}
}
