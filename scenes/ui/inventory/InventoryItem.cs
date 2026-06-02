using Godot;
using System;
using System.Collections.Generic;
using RPG.scripts;

public enum ItemTypes { None, Consumable, Weapon, Armor, Spell, Seed }
public enum ItemEffects {None, Heal, Damage}

[Tool]
public partial class InventoryItem : Node2D
{
	[Export] public int Quantity = 1;
	[Export] public String ItemName = "";
	[Export] public ItemTypes ItemType { get; set; }
	[Export] public ItemEffects ItemEffect {get; set;}

	[Export]
	public Texture2D ItemTexture
	{
		get => _itemTexture;
		set
		{
			_itemTexture = value;
			UpdateSprite();
		}
	}

	private Texture2D _itemTexture;
	private Sprite2D _sprite;
	private string _scenePath = "res://scenes/ui/inventory/inventory_item.tscn";

	public bool PlayerInRange;
		
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		if (_sprite == null) return; // not ready yet
		_sprite.Texture = _itemTexture;
	}

	private void PickupItem()
	{
		var item = new ItemData
		{
			Quantity = Quantity,
			Name = ItemName,
			Type = ItemType,
			Effect = ItemEffect,
			Texture = ItemTexture,
			ScenePath = _scenePath
		};
		if (GlobalFunctions.Instance.PlayerNode != null)
		{
			GlobalFunctions.Instance.AddItem(item);
			QueueFree();
		}
	}

	public void _on_area_2d_body_entered(Node2D body)
	{
		if (body.GetGroups().Contains("player"))
		{
			PickupItem();
			GD.Print("Picked up item");
		}
	}
	
}

public class ItemData
{
	public int Quantity { get; set; }
	public string Name  { get; set; }
	public ItemTypes Type  { get; set; }
	public ItemEffects Effect { get; set; }
	public Texture2D Texture { get; set; }
	public string ScenePath { get; set; }
}
