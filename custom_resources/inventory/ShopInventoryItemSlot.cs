using Godot;

namespace RPG.custom_resources.inventory;

[GlobalClass]
public partial class ShopInventoryItemSlot: Resource
{
    [Export] public InventoryItem Item;
    [Export] public int Price;
}