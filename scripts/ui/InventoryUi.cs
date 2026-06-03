using Godot;
using RPG.scripts;
using RPG.scripts.ui;

namespace RPG.scenes.ui.inventory;

public partial class InventoryUi : Control
{
	private GlobalFunctions _global;
	
	private GridContainer _gridContainer;
	private TooltipLayer _tooltipLayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = GlobalFunctions.Instance;
		_gridContainer = GetNode<GridContainer>("TextureRect/TextureRect/ScrollContainer/CenterContainer/GridContainer");
		_tooltipLayer = GetNode<TooltipLayer>("TooltipLayer");
		GlobalFunctions.Instance.PlayerInventoryUpdated += _on_inventory_updated;
		SetInventory(_global.PlayerInventory);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_tooltipLayer.Visible = Visible;
		if (!Visible && _tooltipLayer.HasTooltip())
		{
			_tooltipLayer.ClearTooltip();
		}
	}

	private void _on_inventory_updated(Inventory inventory)
	{
		SetInventory(inventory);
	}

	private void SetInventory(Inventory inventory)
	{
		ClearGridContainer();

		if (inventory != null)
		{
			foreach (var item in inventory.Items)
			{
				var slot = _global.InventorySlotScene.Instantiate<InventorySlot>();
				_gridContainer.AddChild(slot);
				if (item != null)
				{
					slot.Init(_tooltipLayer);
					slot.SetItem(item);
				}
				else
				{
					slot.SetEmpty();
				}
			}
		}
	}

	private void ClearGridContainer()
	{
		_tooltipLayer.ClearTooltip();
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
