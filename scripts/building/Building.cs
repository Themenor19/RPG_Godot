using Godot;
using Godot.Collections;

namespace RPG.scripts.building;

[Tool]
public abstract partial class Building : StaticBody2D
{
	private bool _snapNodeToGrid;

	public enum ItemRotation
	{
		Normal,
		Reverse,
		Clockwise,
		Counterclockwise
	}
	

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
	
	private ItemRotation _itemRotation = ItemRotation.Normal;
	[Export] public new ItemRotation Rotation{
		get => _itemRotation;
		set
		{
			_itemRotation = value;
			HandleRotation();
		}
	}
	
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

	public abstract void HandleRotation();

	public override void _Notification(int what)
	{
		base._Notification(what);
		
		if (what == NotificationTransformChanged && Engine.IsEditorHint() && SnapNodeToGrid)
		{
			SnapToGrid();
		}
	}

	private bool _snapping;
	public void SnapToGrid()
	{
		if (_snapping) return;
		GD.Print($"Snapping {Name}");
		TileMapLayer gridLayer = null;
		if (GridLayer != null) gridLayer = GridLayer;
		if (_mainGridLayer != null) gridLayer = _mainGridLayer;
		if (gridLayer == null) return;
		
		var localDoorCoords = gridLayer.ToLocal(GlobalPosition);
		var snapCoords = gridLayer.MapToLocal(gridLayer.LocalToMap(localDoorCoords));
		var target = gridLayer.ToGlobal(snapCoords);

		if (Mathf.Abs(GlobalPosition.X - target.X) <= .001 && Mathf.Abs(GlobalPosition.Y - target.Y) <= .001) return;
		
		_snapping = true;
		GlobalPosition = gridLayer.ToGlobal(snapCoords);
		_snapping = false;
	}
}
