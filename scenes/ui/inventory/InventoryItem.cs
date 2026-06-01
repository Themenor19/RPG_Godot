using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class InventoryItem : Node2D
{
	public enum ItemTypes { None, Consumable, Weapon, Armor, Spell, Seed }
	public enum ItemEffects {None, Heal, Damage}

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
		var item = new Dictionary<string, dynamic>
		{
			["quantity"] = 
		} 
	}
	
	
}
