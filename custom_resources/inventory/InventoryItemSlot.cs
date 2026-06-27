using Godot;

namespace RPG.custom_resources.inventory;

[GlobalClass] 
public partial class InventoryItemSlot : Resource
{
	[Export] public InventoryItem Item { get; set; }
	[Export] public int Quantity { get; set; } = 1;

	public InventoryItemSlot() { }

	public InventoryItemSlot(InventoryItem item, int quantity)
	{
		Item = item;
		Quantity = quantity;
	}
}
