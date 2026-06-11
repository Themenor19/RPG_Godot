using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Godot.Collections;

public enum ItemTypes { Consumable, Weapon, Armor, Spell, Seed, Tool, Coin}
public enum ItemEffects { None, Heal, Damage, Hoes, Cast}
public enum ToolTypes {Default, Hoe, WateringCan, Pickaxe}

[Tool]
[GlobalClass]
public partial class InventoryItem : Resource
{
	[Export] 
	public int Id 
	{ 
		get => _id; 
		set => _id = value;
	}
	private int _id;
	[Export] public int Value;
	[Export] public int Quantity = 1;
	[Export] public string Name = "";

	private ItemTypes _type;

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
	//Scene used as an instantiater for things such as spells or plants. 
	[Export] public PackedScene ItemScene;
	[Export] public Texture2D Icon;

	[Export] public ToolTypes ToolType { get; set; } =  ToolTypes.Default;
	[Export] public int HealAmount;
	[Export] public int Damage {get; set;}

	[Export] public string Description { get; set; }

	private ItemEffects _effect;

	public InventoryItem()
	{
		GD.Print("Constructor called");
		if (Engine.IsEditorHint())
			AssignNextId();
	}

	public override void _Notification(int what)
	{
		GD.Print($"Notification: {what}, PostInit={NotificationPostinitialize}");
		if (what == NotificationPostinitialize && Engine.IsEditorHint())
		{
			AssignNextId();
		}
	}

	private void AssignNextId()
	{
		if (_id != 0) return; // Already has an ID, skip

		var dir = DirAccess.Open("res://assets/items/");
		if (dir == null)
		{
			GD.Print("Directory not found");
			return;
		}

		int maxId = 0;
		dir.ListDirBegin();
		string fileName = dir.GetNext();

		while (fileName != "")
		{
			if (fileName.EndsWith(".tres"))
			{
				string path = ProjectSettings.GlobalizePath("res://assets/items/" + fileName);
				GD.Print($"Reading file: {path}");
				string[] lines = File.ReadAllLines(path);
				foreach (string line in lines)
				{
					GD.Print($"  Line: '{line}'");
					string trimmed = line.Trim();
					if (trimmed.StartsWith("Id = "))
					{
						GD.Print($"  Found Id line: {trimmed}");
						if (int.TryParse(trimmed.Substring(5).Trim(), out int id))
						{
							GD.Print($"  Parsed id: {id}");
							if (id > maxId) maxId = id;
						}
					}
				}
			}
			fileName = dir.GetNext();
		}

		GD.Print($"Final maxId: {maxId}, assigning: {maxId + 1}");
		_id = maxId + 1;
		NotifyPropertyListChanged();
	}

	public override void _ValidateProperty(Dictionary property)
	{
		base._ValidateProperty(property);
		var propertyName =  property["name"].AsStringName();
		if (propertyName == PropertyName.Damage)
		{
			if ((Type != ItemTypes.Weapon && Type != ItemTypes.Spell) || Effect != ItemEffects.Damage)
			{
				property["usage"] = (int)(PropertyUsageFlags.NoEditor);
			}
		}
		
		if (propertyName == PropertyName.HealAmount)
		{
			if (Type is not ItemTypes.Spell and not ItemTypes.Consumable || Effect != ItemEffects.Heal)
			{
				property["usage"] = (int)(PropertyUsageFlags.NoEditor);
			}
		}

		if (propertyName == PropertyName.ToolType || propertyName == PropertyName.Effect)
		{
			if (Type is not ItemTypes.Tool)
			{
				property["usage"] = (int)(PropertyUsageFlags.NoEditor);
			}
		}

		if (propertyName == PropertyName.ItemScene)
		{
			if (Type is not ItemTypes.Spell and not ItemTypes.Seed)
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
