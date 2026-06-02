using Godot;
using System;

public partial class InventorySlot : Control
{
    private TextureRect _icon;
    private Label _quantity;
    private NinePatchRect _detailsPanel;
    private Label _itemName;
    private Label _itemType;
    private Label _itemEffect;
    private NinePatchRect _usagePanel;
    
    public void _on_item_button_mouse_entered()
    {
        _icon = GetNode<TextureRect>("ItemPanel/ItemIcon");
        _quantity = GetNode<Label>("ItemPanel/ItemQuantity");
        _itemName = GetNode<Label>("00");
    }

    public void _on_item_button_mouse_exited()
    {
        
    }

    public void _on_item_button_pressed()
    {
        
    }
}
