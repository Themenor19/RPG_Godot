using Godot;
using RPG.custom_resources.inventory;

namespace RPG.scripts.ui;

public partial class ShopSlot : Control
{
	[Export] public Texture2D EmptyTexture;
	[Export] public Texture2D FullTexture;
	[Export] public Texture2D MousedOverTexture;
	[Export] public TextureRect ItemPanel;
	[Export] public Button ItemButton;
	[Export] public TextureRect ItemIcon;
	[Export] public Label QuantityLabel;
	
	[Signal] public delegate void ItemClickedEventHandler(ShopInventoryItemSlot item);
	
	private ShopInventoryItemSlot _item;
	private int _slotNumber;
	private bool _canClick = true;

	public void SetItem(ShopInventoryItemSlot item, int slotNumber = -1)
	{
		if (item == null)
		{
			_item = null;
			_slotNumber = -1;
			SetEmpty();
			_canClick = false;
		}
		else
		{
			_item = item;
			_slotNumber = slotNumber;
			ItemPanel.Texture = FullTexture;
			ItemIcon.Visible = true;
			ItemIcon.Texture = _item.Item.Icon;
			QuantityLabel.Visible = true;
			QuantityLabel.Text = _item.Price.ToString();
			_canClick = true;
		}
	}

	public void SetEmpty()
	{
		ItemPanel.Texture = EmptyTexture;
		ItemIcon.Visible = false;
		QuantityLabel.Visible = false;
		_canClick = false;
	}
	
	private void _on_item_button_mouse_entered()
	{
		if (!_canClick)
		{
			ItemPanel.Texture = EmptyTexture;
			return;
		}
		ItemPanel.Texture = MousedOverTexture;
	}

	private void _on_item_button_mouse_exited()
	{
		if (!_canClick)
		{
			ItemPanel.Texture = EmptyTexture;
			return;
		}
		ItemPanel.Texture = FullTexture;
	}

	private void _on_item_button_clicked()
	{
		EmitSignal(SignalName.ItemClicked, _item);
	}
	
	public override void _EnterTree()
	{
		base._EnterTree();
		ItemButton.MouseEntered += _on_item_button_mouse_entered;
		ItemButton.MouseExited += _on_item_button_mouse_exited;
		ItemButton.Pressed += _on_item_button_clicked;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		ItemButton.MouseEntered -= _on_item_button_mouse_entered;
		ItemButton.MouseExited -= _on_item_button_mouse_exited;
		ItemButton.Pressed -= _on_item_button_clicked;
	}
}
