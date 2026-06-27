using Godot;
using RPG.custom_resources.inventory;

namespace RPG.scripts.ui;

public partial class InventoryItemSelectionLayer : CanvasLayer
{
	[Export] private Sprite2D _sprite;

	private InventoryItemSlot _item;
	private int _slotIndex;
	
	public bool ItemSelected;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (ItemSelected && _sprite.Texture != null)
		{
			_sprite.GlobalPosition = GetViewport().GetMousePosition();
		}
	}

	public void AddItemToSelection(InventoryItemSlot item, int slotIndex)
	{
		_item = item;
		_slotIndex = slotIndex;
		_sprite.Texture = _item.Item.Icon;
		ItemSelected = true;
	}

	public InventoryItemSlot TransferItem()
	{
		if (!ItemSelected || _item == null || _sprite == null) return null;
		ItemSelected = false;
		_sprite.Texture = null;
		var item = _item;
		_item = null;
		return item;
	}

	public int GetSlotIndex()
	{
		return _slotIndex;
	}
}
