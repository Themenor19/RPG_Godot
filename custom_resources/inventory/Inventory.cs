using Godot;
using Godot.Collections;

namespace RPG.custom_resources.inventory;

[GlobalClass]
public partial class Inventory : Resource
{
	[Export]
	public Array<InventoryItemSlot> Items { get; set; } = new();
}
