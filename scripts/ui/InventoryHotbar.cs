using Godot;
using RPG.custom_resources.inventory;
using RPG.scenes.ui.inventory;

namespace RPG.scripts.ui;

public partial class InventoryHotbar : Control
{
	private int _slotSelected = -1;
	
	[Export] private HBoxContainer _hotbarContainer;
	private Global _global;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = Global.Instance;
		_global.PlayerInventoryUpdated += UpdateHotbar;
		UpdateHotbar(_global.HotbarInventory, _global.PlayerInventory);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void UpdateHotbar(Inventory hotbarInventory, Inventory playerInventory)
	{
		ClearHotbar();
		for (var i = 0; i < hotbarInventory.Items.Length; i++)
		{
			var slot = _global.HotbarSlotScene.Instantiate<HotbarSlot>();
			slot.SetIndex(i);
			var item = hotbarInventory.Items[i];
			if (item == null)
			{
				slot.SetEmpty();
			}
			else
			{
				slot.SetItem(item);
			}
			_hotbarContainer.AddChild(slot);
		}

		SetHotbarSelected(_slotSelected);
	}

	private void ClearHotbar()
	{
		foreach (var item in _hotbarContainer.GetChildren())
		{
			_hotbarContainer.RemoveChild(item);
			item.QueueFree();
		}
	}

	public void CheckHotbarSelected(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= _hotbarContainer.GetChildCount() || slotIndex == _slotSelected)
		{
			SetHotbarSelected(-1);
		}
		else
		{
			SetHotbarSelected(slotIndex);
		}
	}
	
	public void SetHotbarSelected(int slotIndex)
	{
		_slotSelected = slotIndex;
		ResetSelectedSlots();
		if (_slotSelected == -1)
		{
			return;
		}
		var item = _hotbarContainer.GetChild(slotIndex) as HotbarSlot;
		item?.SetSelected();
	}

	public void ResetSelectedSlots()
	{
		foreach (var item in _hotbarContainer.GetChildren())
		{
			var hotbarSlot = item as HotbarSlot;
			if (hotbarSlot == null) continue;
			hotbarSlot.SetUnselected();
		}
	}
}
