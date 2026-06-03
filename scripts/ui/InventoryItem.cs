using Godot;
using System;
using Godot.Collections;

public enum ItemTypes { None, Consumable, Weapon, Armor, Spell, Seed }
public enum ItemEffects { None, Heal, Damage }

[Tool] 
[GlobalClass]
public partial class InventoryItem : Resource
{
	[Export] public int Quantity = 1;
	[Export] public string Name = "";

	[Export]
	public ItemTypes Type
	{
		get => _type;
		set
		{
			_type = value;
			NotifyPropertyListChanged();
		}
	}

	[Export]
	public ItemEffects Effect
	{
		get => _effect;
		set
		{
			_effect = value;
			NotifyPropertyListChanged();
		}
	}
	[Export] public Texture2D Icon;
	[Export] public int Damage {get; set;}
	
	private ItemTypes _type;
	private ItemEffects _effect;

	public InventoryItem() { }

	public override void _ValidateProperty(Dictionary property)
	{
		base._ValidateProperty(property);
		if (property["name"].AsStringName() == PropertyName.Damage)
		{
			if ((Type != ItemTypes.Weapon && Type != ItemTypes.Spell) || Effect != ItemEffects.Damage)
			{
				property["usage"] = (int)(PropertyUsageFlags.NoEditor);
			}
		}
	}
}

public abstract class ItemData
{
	public int Quantity { get; set; }
	public string Name { get; set; }
	public ItemTypes Type { get; set; }
	public ItemEffects Effect { get; set; }
	public Texture2D Texture { get; set; }
	public string ScenePath { get; set; }
}
