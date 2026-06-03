using Godot;

namespace RPG.scripts.ui;

[Tool]
public partial class WorldInventoryItem : Node2D
{
	private Global _global;
	
	// 1. Export as base Resource so the Godot Editor UI accepts the drag-and-drop
	[Export] public InventoryItem ItemResource;

	private Sprite2D _sprite;

	public override void _Ready()
	{
		_global = Global.Instance;
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		Scale = Vector2.One * .5f;
	}

	public override void _Process(double delta)
	{
		if (_sprite == null || ItemResource == null) return;

		if (_sprite.Texture != ItemResource.Icon)
		{
			_sprite.Texture = ItemResource.Icon;
		}

	}

	private void _on_area_2d_body_entered(Node2D body)
	{
		if (body is Player)
		{
			if (_global.AddItem(ItemResource))
			{
				QueueFree();
			}
		}
	}
	
}
