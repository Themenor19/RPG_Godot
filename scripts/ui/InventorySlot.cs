using Godot;
using RPG.custom_resources.inventory;
using RPG.scripts.ui;
using Global = RPG.scripts.globals.Global;

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

	public InventoryItemSlot Item => _item;

	private InventoryItemSlot _item;

	[Export] private Texture2D _emptyTexture;
	[Export] private Texture2D _fullTexture;
	[Export] private Texture2D _mousedOverTexture;
	private TooltipLayer _tooltipLayer;
	private InventoryItemSelectionLayer _inventoryItemSelectionLayer;
	private InventoryUi _inventoryUi;
	
	private bool _isShowingDetails;
	public bool UsagePanelOpen;
	private bool _mouseInBox;
	private bool _itemSlotDragging;
	private bool _ignoreNextRightClick;

	public int SlotIndex = -1;
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
		SlotIndex = slotIndex;
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

	public override void _Input(InputEvent @event)
	{
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
		if (!UsagePanelOpen)
		{
			_inventoryUi.UnsetNodeAsSelected(this);
		}
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
	public void SetItem(InventoryItemSlot item)
	{
		_itemPanel.Texture = _fullTexture;
		_item = item;
		_icon.Texture = _item.Item.Icon;
		_quantity.Text = _item.Quantity.ToString();
		_itemName.Text = _item.Item.Name;
		_itemType.Text = $"{item.Item.Type}";
		_itemEffect.Text = $"{item.Item.Effect}";
	}

	private void _on_use_button_pressed()
	{
		GD.Print("Use Button pressed");
		UsagePanelOpen = false;
		_global.UseItem(_item, SlotIndex);
		_tooltipLayer.ClearTooltip();
	}
	
	private void _on_drop_button_pressed()
	{
		UsagePanelOpen = false; 
		if (_item == null) return;
		var dropOffset = _global.GetDropOffset(50f);
		_global.DropItem(_item, SlotIndex, dropOffset, 1);
	}
	
}
