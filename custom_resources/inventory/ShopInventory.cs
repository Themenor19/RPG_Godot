using Godot;
using Godot.Collections;

namespace RPG.custom_resources.inventory;

[GlobalClass]
public partial class ShopInventory: Resource
{
    [Export] public Array<ShopInventoryItemSlot> Items;
}