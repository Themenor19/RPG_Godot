using System.Linq;
using Godot;
using RPG.custom_resources.inventory;
using RPG.scripts.globals;

namespace RPG.scripts.ui;

public partial class Shop : Control
{
	[Export] public ShopInventory ShopInventory;
	[Export] public ShopMenu ShopMenu;
	[Export] public InventoryUi PlayerInventory;
	[Export] public Label CurrentCoinsAmount;
	[Export] public Button NextButton;
	[Export] public Button BackButton;
	[Export] public BaseButton ExitButton;

	private GlobalHandler _global;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ShopMenu.Init(ShopInventory);
		ShopMenu.OnItemSelected = BuyItem;
		if (_global != null)
		{
			CurrentCoinsAmount.Text = _global.CoinAmount.ToString();
		}
	}

	private void BuyItem(ShopInventoryItemSlot item)
	{
		if (_global.CoinAmount >= item.Price)
		{
			_global.SetCoinAmount(_global.CoinAmount - item.Price);
			_global.AddItemToPlayer(new InventoryItemSlot
			{
				Item = item.Item,
				Quantity = 1
			}, InventoryToAdd.Inventory);
		}
	}

	private void _on_inventory_updated(Inventory hotbar, Inventory playerInventory)
	{
		
	}

	private void _on_coin_amount_changed(int amount)
	{
		CurrentCoinsAmount.Text = amount.ToString();
	}

	private void _on_next_button_pressed()
	{
		ShopMenu.Next();
	}

	private void _on_back_button_pressed()
	{
		ShopMenu.Prev();
	}

	private void _on_exit_button_pressed()
	{
		_global.PopLevelAndMove();
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		_global?.CoinAmountChanged += _on_coin_amount_changed;
		_global?.PlayerInventoryUpdated += _on_inventory_updated;
		NextButton.Pressed += _on_next_button_pressed;
		BackButton.Pressed += _on_back_button_pressed;
		ExitButton.Pressed += _on_exit_button_pressed; 
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_global?.CoinAmountChanged -= _on_coin_amount_changed;
		_global?.PlayerInventoryUpdated -= _on_inventory_updated;
		NextButton.Pressed -= _on_next_button_pressed;
		BackButton.Pressed -= _on_back_button_pressed;
		ExitButton.Pressed -= _on_exit_button_pressed;
	}
}
