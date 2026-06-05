using System;
using Godot;
using RPG.scripts;
using RPG.scripts.ui;

namespace RPG.scenes.ui.inventory;

public partial class InventoryUi : Control
{
	private Global _global;
	
	private GridContainer _gridContainer;
	private TooltipLayer _tooltipLayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = Global.Instance;
		_gridContainer = GetNode<GridContainer>("TextureRect/TextureRect/ScrollContainer/CenterContainer/GridContainer");
		_tooltipLayer = GetNode<TooltipLayer>("TooltipLayer");
		Global.Instance.PlayerInventoryUpdated += _on_inventory_updated;
		
		ClearGridContainer();
		// Create slots once
		for (int i = 0; i < _global.PlayerInventory.Items.Length; i++)
		{
			var slot = _global.InventorySlotScene.Instantiate<InventorySlot>();
			_gridContainer.AddChild(slot);
			slot.Init(_tooltipLayer, i);
		}

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

	private void _on_inventory_updated(custom_resources.inventory.Inventory inventory)
	{
		_tooltipLayer.ClearTooltip();
		SetInventory(inventory);
	}

	private void SetInventory(custom_resources.inventory.Inventory inventory)
	{
		if (inventory == null) return;

		var items = inventory.Items;

		for (int i = 0; i < _gridContainer.GetChildCount(); i++)
		{
			var slot = _gridContainer.GetChild<InventorySlot>(i);
			if (i < items.Length && items[i] != null)
				slot.SetItem(items[i]);
			else
				slot.SetEmpty();
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
		Global.Instance.PlayerInventoryUpdated -= _on_inventory_updated;
	}
}
