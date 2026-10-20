using Godot;
using Godot.Collections;
using RPG.custom_resources.inventory;

namespace RPG.scripts.ui;

public partial class ShopMenu : Control
{
	[Export] public PackedScene SlotPrefab;
	[Export] public GridContainer GridContainer;
	[Export] public int NumberOfSlots;
	
	private Inventory _inventory;
	
	private Array<ShopSlot> _slots;

	public void Init(Inventory inventory)
	{
		_inventory = inventory;
	}

	public override void _Ready()
	{
		for (int i = 0; i < NumberOfSlots; i++)
		{
			var slot = SlotPrefab.Instantiate<ShopSlot>();
			GridContainer.AddChild(slot);
			slot.
		}
	}
}
