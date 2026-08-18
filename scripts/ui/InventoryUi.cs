using System.Linq;
using Godot;
using RPG.custom_resources.inventory;
using RPG.scenes.ui.inventory;
using RPG.scripts.globals;

namespace RPG.scripts.ui;

public partial class InventoryUi : Control
{
	private GlobalHandler _global;
	
	[Export] private InventoryItemSelectionLayer _inventoryItemSelectionLayer;
	private GridContainer _gridContainer;
	private TooltipLayer _tooltipLayer;

	private InventorySlot _selectedSlot;
	private NinePatchRect _activeDetailsPanel;
	private NinePatchRect _activeUsagePanel;

	private bool _usagePanelShown;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gridContainer = GetNode<GridContainer>("TextureRect/TextureRect/ScrollContainer/CenterContainer/GridContainer");
		_tooltipLayer = GetNode<TooltipLayer>("TooltipLayer");
		if (_global != null)
		{
			ClearGridContainer();
			// Create slots once
			for (int i = 0; i < _global.PlayerInventory.Items.Count; i++)
			{
				var slot = _global.InventorySlotScene.Instantiate<InventorySlot>();
				_gridContainer.AddChild(slot);
				slot.Init(_tooltipLayer, _inventoryItemSelectionLayer, i, _gridContainer, this);
			}

			SetInventory(_global.PlayerInventory);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_tooltipLayer.Visible = Visible;

		if (!Visible && _tooltipLayer.HasTooltip())
		{
			_tooltipLayer.ClearTooltip();
			_activeDetailsPanel = null;
			_selectedSlot = null;
		}

		if (Visible && _activeDetailsPanel != null && _activeDetailsPanel.Visible)
		{
			_activeDetailsPanel.GlobalPosition = _tooltipLayer.GetViewport().GetMousePosition() 
				- _activeDetailsPanel.Size / 2f + Vector2.Up * 32;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true } && _selectedSlot != null)
		{
			ShowUsagePanel();
		}
		else if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false } && _selectedSlot != null && !_usagePanelShown)
		{
			ItemClicked();
		}
	}
	
	private void ItemClicked()
	{
		if (_selectedSlot == null) return;
		if (_inventoryItemSelectionLayer.ItemSelected && _selectedSlot  != null)
		{
			_selectedSlot.SetItem(_inventoryItemSelectionLayer.TransferItem());
			var selectedIndex = _selectedSlot.SlotIndex;
			_global.SwapItems(_selectedSlot.SlotIndex, _inventoryItemSelectionLayer.GetSlotIndex());
			var item = _gridContainer.GetChild(selectedIndex) as InventorySlot;
			SetNodeAsSelected(item);
		}
		else
		{
			_inventoryItemSelectionLayer.AddItemToSelection(_selectedSlot.Item, _selectedSlot.SlotIndex);
			_selectedSlot.SetEmpty();
		}
	}
	
	private void _on_inventory_updated(Inventory hotbar, Inventory playerInventory)
	{
		_tooltipLayer.ClearTooltip();
		_activeDetailsPanel = null;
		_selectedSlot = null;
		SetInventory(playerInventory);
	}

	private void SetInventory(Inventory inventory)
	{
		if (inventory == null) return;

		var items = inventory.Items;
		int slotIndex = 0;

		for (int i = 0; i < _gridContainer.GetChildCount(); i++)
		{
			// Skip anything that isn't an InventorySlot
			if (_gridContainer.GetChild(i) is not InventorySlot slot) continue;

			if (slotIndex < items.Count && items[slotIndex] != null)
				slot.SetItem(items[slotIndex]);
			else
				slot.SetEmpty();

			slotIndex++;
		}
	}

	
	public void SetNodeAsSelected(InventorySlot slot)
	{
		if (_selectedSlot != null && _selectedSlot != slot)
		{
			_selectedSlot.SetUnselected();
			_tooltipLayer.ClearTooltip();
		}

		_selectedSlot = slot;
		_selectedSlot.SetSelected();

		if (slot.Item == null) return;

		_tooltipLayer.AddTooltip(slot.DetailsPanel, _gridContainer);
		slot.DetailsPanel.Visible = true;
		_activeDetailsPanel = slot.DetailsPanel;
		_activeUsagePanel = slot.UsagePanel;
	}

	public void UnsetNodeAsSelected(InventorySlot slot)
	{
		if (_selectedSlot == null || _selectedSlot != slot) return;
		_selectedSlot.SetUnselected();
		_selectedSlot = null;
		_activeDetailsPanel = null;
		_tooltipLayer.ClearTooltip();
	}

	public void ShowUsagePanel()
	{
		_selectedSlot.UsagePanelOpen = true;
		_usagePanelShown = true;
		_tooltipLayer.AddTooltip(_activeUsagePanel, _gridContainer);
		_activeUsagePanel.GlobalPosition = _tooltipLayer.GetViewport().GetMousePosition() + Vector2.Right * 2;
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
		_global.PlayerInventoryUpdated -= _on_inventory_updated;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		if (_global != null) _global.PlayerInventoryUpdated += _on_inventory_updated;
	}
}
