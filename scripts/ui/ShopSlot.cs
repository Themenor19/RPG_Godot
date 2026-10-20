using Godot;

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
	
	private InventoryItem _item;

	public bool SetItem(InventoryItem item)
	{
		_item = item;
		return _item != null;
	}
	
	private void _on_item_button_mouse_entered()
	{
		ItemPanel.Texture = MousedOverTexture;
	}

	private void _on_item_button_mouse_exited()
	{
		ItemPanel.Texture = FullTexture;
	}
	
	public override void _EnterTree()
	{
		base._EnterTree();
		ItemButton.MouseEntered += _on_item_button_mouse_entered;
		ItemButton.MouseExited += _on_item_button_mouse_exited;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		ItemButton.MouseEntered -= _on_item_button_mouse_entered;
		ItemButton.MouseExited -= _on_item_button_mouse_exited;
	}
}
