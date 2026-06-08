using Godot;
using System;
using System.IO;
using Godot.Collections;

public enum ItemTypes { None, Consumable, Weapon, Armor, Spell, Seed, Tool}
public enum ItemEffects { None, Heal, Damage, Hoes}
public enum ToolTypes {Default, Hoe, WateringCan, Pickaxe}

[Tool]
[GlobalClass]
public partial class InventoryItem : Resource
{
	[Export] 
	public int Id 
	{ 
		get => _id;
		private set => _id = value;
	}
	private int _id;
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

	[Export] public ToolTypes ToolType { get; set; } =  ToolTypes.Default;
	[Export] public int Damage {get; set;}
	
	private ItemTypes _type;
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
				string[] lines = System.IO.File.ReadAllLines(path);
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
		if (property["name"].AsStringName() == PropertyName.Damage)
		{
			if ((Type != ItemTypes.Weapon && Type != ItemTypes.Spell) || Effect != ItemEffects.Damage)
			{
				property["usage"] = (int)(PropertyUsageFlags.NoEditor);
			}
		}

		if (property["name"].AsStringName() == PropertyName.ToolType)
		{
			if (Type != ItemTypes.Tool)
			{
				property["usage"] = (int)(PropertyUsageFlags.NoEditor);
			}
		}

		if (property["name"].AsStringName() == PropertyName.Effect)
		{
			if (Type == ItemTypes.Tool)
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
