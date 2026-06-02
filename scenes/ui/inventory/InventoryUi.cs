using Godot;
using RPG.scripts;

namespace RPG.scenes.ui.inventory;

public partial class InventoryUi : Control
{
	private GridContainer _gridContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gridContainer = GetNode<GridContainer>("TextureRect/CenterContainer/MarginContainer/ScrollContainer/CenterContainer/GridContainer");
		GlobalFunctions.Instance.PlayerInventoryUpdated += _on_inventory_updated;
		ClearGridContainer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_inventory_updated()
	{
		
	}

	private void ClearGridContainer()
	{
		while (_gridContainer.GetChildCount() > 0)
		{
			var child = _gridContainer.GetChild(0);
			_gridContainer.RemoveChild(child);
			child.QueueFree();
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		GlobalFunctions.Instance.PlayerInventoryUpdated -= _on_inventory_updated;
	}
}
