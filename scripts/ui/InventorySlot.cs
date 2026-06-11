using Godot;
using RPG.scripts;
using RPG.scripts.ui;

namespace RPG.scenes.ui.inventory;

public partial class InventorySlot : Control
{
	private TextureRect _icon;
	private TextureRect _itemPanel;
	private Label _quantity;
	public NinePatchRect DetailsPanel;
	private Label _itemName;
	private Label _itemType;
	private Label _itemEffect;
	public NinePatchRect UsagePanel;

	public InventoryItem Item => _item;

	private InventoryItem _item;

	[Export] private Texture2D _emptyTexture;
	[Export] private Texture2D _fullTexture;
	[Export] private Texture2D _mousedOverTexture;
	private TooltipLayer _tooltipLayer;
	private InventoryItemSelectionLayer _inventoryItemSelectionLayer;
	private InventoryUi _inventoryUi;
	
	private bool _isShowingDetails;
	private bool _usagePanelOpen;
	private bool _mouseInBox;
	private bool _itemSlotDragging;
	private bool _ignoreNextRightClick;

	private int _slotIndex = -1;
	private Control _originalParent;

	private Global _global;
	
	
	public override void _Ready()
	{
		_global = Global.Instance;
		_itemPanel = GetNode<TextureRect>("ItemPanel");
		_icon = _itemPanel.GetNode<TextureRect>("ItemIcon");
		_quantity = _itemPanel.GetNode<Label>("ItemQuantity");
		DetailsPanel = GetNode<NinePatchRect>("DetailsPanel");
		_itemName = DetailsPanel.GetNode<Label>("Margins/ItemName");
		_itemType = DetailsPanel.GetNode<Label>("Margins/ItemType");
		_itemEffect = DetailsPanel.GetNode<Label>("Margins/ItemEffect");
		UsagePanel = GetNode<NinePatchRect>("UsagePanel");

		DetailsPanel.Visible = false;
		
	}

	public void Init(TooltipLayer tooltipLayer, InventoryItemSelectionLayer inventoryItemSelectionLayer, int slotIndex, Control originalParent, InventoryUi inventoryUi)
	{
		_inventoryItemSelectionLayer =  inventoryItemSelectionLayer;
		_tooltipLayer = tooltipLayer;
		DetailsPanel.Visible = false;
		_slotIndex = slotIndex;
		UsagePanel.Visible = false;
		_originalParent = originalParent;
		_inventoryUi = inventoryUi;
	}

	

	public override void _Process(double delta)
	{
		
		if (DetailsPanel.Visible)
		{
			DetailsPanel.GlobalPosition = _tooltipLayer.GetViewport().GetMousePosition() - DetailsPanel.Size / 2f + Vector2.Up * 32;
		}
	}

	private void UsagePanelPressed()
	{
		if (_item == null) return;

		if (_usagePanelOpen)
		{
			GD.Print("Usage panel was open");
			_usagePanelOpen = false;
			_tooltipLayer.ClearTooltip();
			DetailsPanel.Visible = false;
			return;
		}
		
		_usagePanelOpen = true;
		GD.Print($"Usage Panel Open: {_usagePanelOpen}");
		_tooltipLayer.AddTooltip(UsagePanel, _originalParent);
		UsagePanel.GlobalPosition = _tooltipLayer.GetViewport().GetMousePosition() + Vector2.Right * 2;
		DetailsPanel.Visible = false;
	}

	private void ItemClicked()
	{
		if (_inventoryItemSelectionLayer.ItemSelected && _mouseInBox)
		{
			SetItem(_inventoryItemSelectionLayer.TransferItem());
			_global.SwapItems(_slotIndex, _inventoryItemSelectionLayer.GetSlotIndex());
		}
		else
		{
			_inventoryItemSelectionLayer.AddItemToSelection(_item, _slotIndex);
			SetEmpty();
			_mouseInBox = true;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true } && _mouseInBox)
		{
			GD.Print($"Mouse in box: {_mouseInBox}");
			if (_mouseInBox)
			{
				if (_ignoreNextRightClick)
				{
					_ignoreNextRightClick = false;
					return;
				}
				UsagePanelPressed();
			}
		}
		else if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false } && _mouseInBox)
		{
			ItemClicked();
		}
	}

	public void SetSelected()
	{
		if (_item != null)
		{
			_itemPanel.Texture = _mousedOverTexture;
		}
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

	private void _on_item_button_mouse_entered()
	{
		_mouseInBox = true;
		_inventoryUi.SetNodeAsSelected(this);
	}

	private void _on_item_button_mouse_exited()
	{
		_mouseInBox = false;
		_inventoryUi.UnsetNodeAsSelected(this);
	}

	//Creates an empty slot
	public void SetEmpty()
	{
		_item = null;
		_itemPanel.Texture = _emptyTexture;
		_icon.Texture = null;
		_quantity.Text = "";
		_itemName.Text = "";
		_itemType.Text = "";
		_itemEffect.Text = "";
		_mouseInBox = false;
		_itemSlotDragging = false;
		UsagePanel.Visible = false;
		_tooltipLayer.ClearTooltip();
	}

	//Set slot item with its values form the Inventory Item
	public void SetItem(InventoryItem item)
	{
		_itemPanel.Texture = _fullTexture;
		_item = item;
		_icon.Texture = _item.Icon;
		_quantity.Text = _item.Quantity.ToString();
		_itemName.Text = _item.Name;
		_itemType.Text = $"{item.Type}";
		_itemEffect.Text = $"{item.Effect}";
	}

	private void _on_use_button_pressed()
	{
		GD.Print("Use Button pressed");
		_usagePanelOpen = false;
		_isShowingDetails = false;
		_ignoreNextRightClick = true; // ✅ next right click is the user trying to reopen, skip it
		_global.UseItem(_item, _slotIndex);
		_tooltipLayer.ClearTooltip();
	}
	
	private void _on_drop_button_pressed()
	{
		_usagePanelOpen = false; 
		if (_item == null) return;
		var dropOffset = GetDropOffset(50f);
		_global.DropItem(_item, _slotIndex, dropOffset, 1);
	}
	

	private Vector2 GetDropOffset(float Offset)
	{
		var playerDirection = _global.PlayerNode.Direction;
		if (playerDirection == LookDirection.North)
		{
			return Vector2.Up*Offset;
		}

		if (playerDirection == LookDirection.South)
		{
			return Vector2.Down*Offset;
		}

		if (playerDirection == LookDirection.East)
		{
			return Vector2.Right * Offset;
		}

		if (playerDirection == LookDirection.West)
		{
			return Vector2.Left * Offset;
		}
		
		return Vector2.Zero;
	}
}
