using Godot;
using RPG.scripts;
using RPG.scripts.ui;

namespace RPG.scenes.ui.inventory;

public partial class InventorySlot : Control
{
	private TextureRect _icon;
	private TextureRect _itemPanel;
	private Label _quantity;
	private NinePatchRect _detailsPanel;
	private Label _itemName;
	private Label _itemType;
	private Label _itemEffect;
	private NinePatchRect _usagePanel;
	
	private InventoryItem _item;

	private Texture2D _emptyTexture;
	private Texture2D _fullTexture;
	private TooltipLayer _tooltipLayer;
	
	private bool _isShowingDetails;

	private int _slotIndex = -1;

	private Global _global;
	
	public override void _Ready()
	{
		_global = Global.Instance;
		_itemPanel = GetNode<TextureRect>("ItemPanel");
		_icon = _itemPanel.GetNode<TextureRect>("ItemIcon");
		_quantity = _itemPanel.GetNode<Label>("ItemQuantity");
		_detailsPanel = GetNode<NinePatchRect>("DetailsPanel");
		_itemName = _detailsPanel.GetNode<Label>("Margins/ItemName");
		_itemType = _detailsPanel.GetNode<Label>("Margins/ItemType");
		_itemEffect = _detailsPanel.GetNode<Label>("Margins/ItemEffect");
		_usagePanel = GetNode<NinePatchRect>("UsagePanel");

		_emptyTexture = GD.Load<Texture2D>("res://assets/Sprites/backgrounds/inventory/un-selected_inventroy_square_v2.png");
		_fullTexture = GD.Load<Texture2D>("res://assets/Sprites/backgrounds/inventory/inventroy_square_v2.png");

		_detailsPanel.Visible = false;
		
	}

	public void Init(TooltipLayer tooltipLayer, int slotIndex)
	{
		_tooltipLayer = tooltipLayer;
		_detailsPanel.Visible = false;
		_slotIndex = slotIndex;
		_usagePanel.Visible = false;
	}

	

	public override void _Process(double delta)
	{
		if (_detailsPanel.Visible)
		{
			_detailsPanel.GlobalPosition = GetViewport().GetMousePosition() - _detailsPanel.Size / 2f + Vector2.Up*32;
		}
	}

	private void _on_item_button_pressed()
	{
		if (_item == null) return;
		_usagePanel.Visible = !_usagePanel.Visible;
		if (_usagePanel.Visible)
		{
			_tooltipLayer.AddTooltip(_usagePanel, _usagePanel.GetParent());
		}
		else
		{
			_tooltipLayer.ClearTooltip();
		}
		_detailsPanel.Visible = false;
		_usagePanel.GlobalPosition = GetViewport().GetMousePosition();
	}



	private void _on_item_button_mouse_entered()
	{
		if (_item == null || _usagePanel.Visible || _isShowingDetails) return;
		_isShowingDetails = true; 
		_tooltipLayer.AddTooltip(_detailsPanel, _detailsPanel.GetParent());
		_detailsPanel.Visible = true;
	}

	private void _on_item_button_mouse_exited()
	{
		if (!_isShowingDetails || _item == null) return;
		_isShowingDetails = false;
		_detailsPanel.Visible = false;
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
		
	}

	private void _on_drop_button_pressed()
	{
		if (_item == null) return;
		var dropOffset = GetDropOffset(50f);
		_global.DropItem(_item, dropOffset);
		_global.RemoveItem(_item, _slotIndex);
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
