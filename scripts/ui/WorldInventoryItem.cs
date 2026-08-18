using System.Linq;
using Godot;
using RPG.custom_resources.inventory;
using RPG.scripts.globals;

namespace RPG.scripts.ui;

[Tool]
public partial class WorldInventoryItem : Node2D
{
	private GlobalHandler _global;
	
	// 1. Export as base Resource so the Godot Editor UI accepts the drag-and-drop
	[Export] public InventoryItemSlot ItemResource;
	[Export] private float _scale = .5f;

	private Sprite2D _sprite;
	private Label _quantityLabel;

	public override void _Ready()
	{
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		_quantityLabel = GetNode<Label>("QuantityLabel");
		_quantityLabel.Text = ItemResource?.Quantity.ToString() ?? "0";
		if (ItemResource == null || ItemResource.Quantity <= 1)
		{
			_quantityLabel.Visible = false;
		}
		else
		{
			_quantityLabel.Visible = true;
		}
		Scale = Vector2.One * _scale;
	}

	public override void _Process(double delta)
	{
		if (_sprite == null || ItemResource == null) return;

		if (_sprite.Texture != ItemResource.Item.Icon)
		{
			_sprite.Texture = ItemResource.Item.Icon;
		}

		Position = Position.Round();
		GlobalPosition = GlobalPosition.Round();
	}

	private void _on_area_2d_body_entered(Node2D body)
	{
		if (body is Player)
		{
			if (_global.AddItemToPlayer(ItemResource, InventoryToAdd.Inventory))
			{
				QueueFree();
			}
		}
	}
	
}
