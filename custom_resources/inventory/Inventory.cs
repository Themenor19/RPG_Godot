using Godot;

namespace RPG.custom_resources.inventory;

[GlobalClass]
public partial class Inventory: Resource
{
	[Export(PropertyHint.ArrayType)]
	public InventoryItem[] Items { get; set; }
}
