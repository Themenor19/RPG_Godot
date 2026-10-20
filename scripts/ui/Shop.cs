using Godot;
using RPG.custom_resources.inventory;

namespace RPG.scripts.ui;

public partial class Shop : Control
{
	[Export] public Inventory ShopInventory;
	[Export] public ShopMenu ShopMenu;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
