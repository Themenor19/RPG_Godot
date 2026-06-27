using Godot;
using RPG.custom_resources.inventory;

namespace RPG.scripts.ui;

public partial class HotbarSlot : Control
{
	[Export] private TextureRect _itemPanel;
	[Export] private TextureRect _icon;
	[Export] private Label _quantity;
	[Export] private Texture2D _emptyTexture;
	[Export] private Texture2D _fullTexture;
	[Export] private Texture2D _selectedTexture;

	private int _slotIndex = -1;
	private InventoryItemSlot _item;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetItem(InventoryItemSlot item)
	{
		_item = item;
		_icon.Texture = _item.Item.Icon;
		_quantity.Text = _item.Quantity.ToString();
		_itemPanel.Texture = _fullTexture;
	}

	public void SetIndex(int slotIndex)
	{
		_slotIndex = slotIndex;
	}

	public void SetEmpty()
	{
		_quantity.Text = "0";
		_icon.Texture = null;
		_item =  null;
		_slotIndex = -1;
		_itemPanel.Texture = _emptyTexture;
	}

	public void SetSelected()
	{
		_itemPanel.Texture = _selectedTexture;
	}
	
	public void SetUnselected()
	{
		if (_item != null)
		{
			_itemPanel.Texture = _fullTexture;
		}
		else
		{
			_itemPanel.Texture = _emptyTexture;
		}
	}
}
